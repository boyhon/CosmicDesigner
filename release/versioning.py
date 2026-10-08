"""VCutting release authority; stdlib only. No version allocation during builds."""
from pathlib import Path
import argparse, hashlib, json, re, subprocess, sys, datetime, xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]

def git(root, *args):
    return subprocess.check_output(['git', *args], cwd=root).decode('utf-8').strip()

def policy(root):
    tree = ET.parse(root / 'release/Version.props')
    return {name: tree.find('.//' + name).text for name in ('DevelopmentVersion', 'TargetCustomerVersion', 'WindowsVersionEpoch', 'InstallerCompilerVersion')}

def sdk(root):
    return subprocess.check_output(['dotnet', '--version'], cwd=root).decode('utf-8').strip()

def numeric(version, epoch):
    m = re.fullmatch(r'(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-rc\.([1-9]\d*))?', version)
    if not m: raise ValueError('Expected MAJOR.MINOR.PATCH[-rc.N]')
    major, minor, patch = map(int, m.group(1, 2, 3))
    revision = int(m[4]) if m[4] else 65535
    values = (major + int(epoch), minor, patch, revision)
    if any(x > 65535 for x in values) or (m[4] and revision >= 65535): raise ValueError('Windows mapping overflow')
    return '.'.join(map(str, values))

def inputs(root):
    raw = subprocess.check_output(['git', 'ls-files', '-c', '-o', '--exclude-standard', '-z'], cwd=root)
    names = sorted(set(x.decode('utf-8') for x in raw.split(b'\0') if x))
    result = {}
    for name in names:
        # Immutable release registry/evidence are not executable source inputs.
        if name.startswith(('release/freezes/', 'release/builds/')): continue
        path = root / name
        if path.is_file(): result[name] = hashlib.sha256(path.read_bytes()).hexdigest()
    return result

def read_record(path):
    envelope = json.loads(Path(path).read_text(encoding='utf-8'))
    record = envelope['record']
    encoded = json.dumps(record, sort_keys=True, ensure_ascii=False).encode()
    if hashlib.sha256(encoded).hexdigest() != envelope['sha256']: raise ValueError('Freeze record integrity mismatch')
    return record

def write_record(path, record):
    path.parent.mkdir(parents=True, exist_ok=True)
    encoded = json.dumps(record, sort_keys=True, ensure_ascii=False).encode()
    with path.open('x', encoding='utf-8') as f:
        json.dump({'record': record, 'sha256': hashlib.sha256(encoded).hexdigest()}, f, ensure_ascii=False, indent=2)

def validate(root, path):
    record = read_record(path)
    expected_path = root / 'release/freezes' / (record['customerVersion'] + '.json')
    if Path(path).resolve() != expected_path.resolve(): raise ValueError('Select the immutable registered Freeze record')
    if git(root, 'cat-file', '-t', record['sourceCommit']) != 'commit': raise ValueError('Freeze source commit missing')
    # Commit the new ledger without rewriting its source anchor. Only ledger changes may follow it.
    post_freeze = git(root, 'diff', '--name-only', record['sourceCommit'], 'HEAD').splitlines()
    if any(not x.startswith(('release/freezes/', 'release/builds/')) for x in post_freeze): raise ValueError('Freeze source commit mismatch')
    current = inputs(root)
    changed = [x for x in sorted(set(current) | set(record['inputs'])) if current.get(x) != record['inputs'].get(x)]
    if changed: raise ValueError('Freeze input mismatch: ' + ', '.join(changed[:20]))
    p = policy(root)
    if record['toolchain'] != {'dotnetSDK': sdk(root), 'innoSetup': p['InstallerCompilerVersion']}: raise ValueError('Freeze toolchain mismatch')
    if record['developmentVersion'] != p['DevelopmentVersion'] or record['numericVersion'] != numeric(record['customerVersion'], p['WindowsVersionEpoch']):
        raise ValueError('Freeze version mapping mismatch')
    if record['buildSettings'] != {'configuration': 'Release', 'runtime': 'win-x64', 'selfContained': True}:
        raise ValueError('Unsupported frozen build settings')
    return record

def approve(path):
    if not path: raise ValueError('Explicit user approval evidence file required')
    text = Path(path).read_text(encoding='utf-8').strip()
    if not text: raise ValueError('Empty approval evidence')
    return {'text': text, 'sha256': hashlib.sha256(text.encode()).hexdigest()}

def allocate(root, approval, promote=None, verification=None):
    consent = approve(approval)
    # All source must exist in the recorded commit. Existing registry files may be untracked.
    dirty = git(root, 'status', '--porcelain', '--untracked-files=all').splitlines()
    if any(not line[3:].startswith(('release/freezes/', 'release/builds/')) for line in dirty):
        raise ValueError('Commit source changes before Freeze; working tree is dirty')
    p = policy(root)
    directory = root / 'release/freezes'
    records = [read_record(x) for x in directory.glob('*.json')]
    if any(x['windowsEpoch'] != int(p['WindowsVersionEpoch']) for x in records): raise ValueError('Windows mapping epoch is immutable after first Freeze')
    base = p['TargetCustomerVersion']
    if any(x['customerVersion'] == base for x in records): raise ValueError('Target already released; approve a new target version')
    evidence = None
    if promote:
        candidate = validate(root, promote)
        if not candidate['customerVersion'].startswith(base + '-rc.'): raise ValueError('Candidate does not match target')
        evidence = json.loads(Path(verification).read_text(encoding='utf-8')) if verification else None
        if not evidence or evidence.get('candidateVersion') != candidate['customerVersion'] or evidence.get('sourceCommit') != candidate['sourceCommit']:
            raise ValueError('Verification must identify exact candidate and source commit')
        for kind in ('automated', 'human'):
            checks = evidence.get(kind, [])
            if not checks or any(x.get('result') != 'PASS' or not x.get('testId') for x in checks): raise ValueError('Required verification incomplete: ' + kind)
        required = json.loads((root / 'tests/regression/manifest.json').read_text(encoding='utf-8'))
        for kind, types in [('automated', ('AUTO',)), ('human', ('MANUAL', 'SEMI_AUTO'))]:
            required_ids = {x['testId'] for x in required['cases'] + required.get('baselineCases', []) if x['status'] == 'ACTIVE' and x['executionType'] in types}
            if not required_ids.issubset({x['testId'] for x in evidence[kind]}): raise ValueError('Missing required verification IDs: ' + kind)
        if any(x['status'] == 'ACTIVE' and x['executionType'] == 'NOT_AUTOMATED' for x in required['cases']): raise ValueError('Required not-automated cases unresolved')
        if any(not x.get('reviewer') or not x.get('observedAt') or not x.get('evidence') for x in evidence['human']): raise ValueError('Human reviewer/time/evidence required')
        version = base
    else:
        numbers = [int(x['customerVersion'].split('-rc.')[1]) for x in records if x['customerVersion'].startswith(base + '-rc.')]
        version = base + '-rc.' + str(max(numbers, default=0) + 1)
    windows = numeric(version, p['WindowsVersionEpoch'])
    if records and tuple(map(int, windows.split('.'))) <= max(tuple(map(int, x['numericVersion'].split('.'))) for x in records):
        raise ValueError('New release version must advance beyond issued versions')
    record = dict(customerVersion=version, developmentVersion=p['DevelopmentVersion'], numericVersion=windows, windowsEpoch=int(p['WindowsVersionEpoch']),
                  sourceCommit=git(root, 'rev-parse', 'HEAD'), inputs=inputs(root), approval=consent,
                  includedCRs=sorted(x.stem for x in (root / 'docs/project/change-requests').glob('CR-*.md')),
                  buildSettings={'configuration': 'Release', 'runtime': 'win-x64', 'selfContained': True},
                  toolchain={'dotnetSDK': sdk(root), 'innoSetup': p['InstallerCompilerVersion']},
                  verification=evidence or {'state': 'PENDING', 'automated': [], 'human': []},
                  promotedFrom=read_record(promote)['customerVersion'] if promote else None,
                  createdAt=datetime.datetime.now(datetime.timezone.utc).isoformat(), artifacts='See append-only release/builds evidence')
    path = directory / (version + '.json')
    write_record(path, record)
    return path

def prepare(root, path, output, build_number):
    record = validate(root, path)
    if not re.fullmatch(r'[A-Za-z0-9_-]+', build_number): raise ValueError('Invalid build number')
    if (root / 'release/builds' / record['customerVersion'] / (build_number + '.json')).exists(): raise ValueError('Build number already recorded; choose a new build identifier')
    output = Path(output).resolve()
    if not output.is_relative_to(root.resolve()) or not output.is_relative_to((root / 'artifacts').resolve()): raise ValueError('Generated build data must be inside repository artifacts')
    output.mkdir(parents=True, exist_ok=True)
    props = ET.Element('Project'); group = ET.SubElement(props, 'PropertyGroup')
    values = {'VCuttingCustomerVersion': record['customerVersion'], 'VCuttingDisplayVersion': record['customerVersion'],
              'VCuttingNumericVersion': record['numericVersion'], 'VCuttingBuildIdentity': record['sourceCommit'] + '.' + build_number,
              'VCuttingFreezeRecord': str(Path(path).resolve())}
    for name, value in values.items(): ET.SubElement(group, name).text = value
    ET.ElementTree(props).write(output / 'Freeze.props', encoding='utf-8', xml_declaration=True)
    (output / 'ReleaseVersion.iss').write_text('\n'.join(f'#define {name} "{value}"' for name, value in {
        'AppVersion': record['customerVersion'], 'AppNumericVersion': record['numericVersion'], 'SetupBaseFilename': 'VCuttingSetup'}.items()) + '\n', encoding='utf-8')
    (output / 'toolchain.json').write_text(json.dumps(record['toolchain']), encoding='utf-8')
    (output / 'build.json').write_text(json.dumps(dict(version=record['customerVersion'], sourceCommit=record['sourceCommit'], buildNumber=build_number,
        numericVersion=record['numericVersion'], recordHash=hashlib.sha256(Path(path).read_bytes()).hexdigest()), indent=2), encoding='utf-8')
    return record

def validate_props(root, path, props):
    record = validate(root, path)
    tree = ET.parse(props)
    expected = {'VCuttingCustomerVersion': record['customerVersion'], 'VCuttingDisplayVersion': record['customerVersion'], 'VCuttingNumericVersion': record['numericVersion'], 'VCuttingFreezeRecord': str(Path(path).resolve())}
    for key, value in expected.items():
        if tree.findtext('.//' + key) != value: raise ValueError('Freeze build property mismatch: ' + key)
    identity = tree.findtext('.//VCuttingBuildIdentity', '')
    if not re.fullmatch(re.escape(record['sourceCommit']) + r'\.[A-Za-z0-9_-]+', identity): raise ValueError('Build identity mismatch')
    return record

def validate_definitions(root, path, definitions):
    record = validate(root, path)
    expected = [f'#define {key} "{value}"' for key, value in {'AppVersion': record['customerVersion'], 'AppNumericVersion': record['numericVersion'], 'SetupBaseFilename': 'VCuttingSetup'}.items()]
    if Path(definitions).read_text(encoding='utf-8').splitlines() != expected: raise ValueError('Generated installer definitions mismatch')
    return record

def record_build(root, path, build_info, artifact):
    freeze = validate(root, path)
    info = json.loads(Path(build_info).read_text(encoding='utf-8'))
    if info['version'] != freeze['customerVersion'] or info['numericVersion'] != freeze['numericVersion'] or info['sourceCommit'] != freeze['sourceCommit'] or info['recordHash'] != hashlib.sha256(Path(path).read_bytes()).hexdigest():
        raise ValueError('Build evidence/Freeze mismatch')
    if not re.fullmatch(r'[A-Za-z0-9_-]+', info['buildNumber']): raise ValueError('Invalid build evidence identifier')
    artifact = Path(artifact)
    directory = root / 'release/builds' / freeze['customerVersion']
    directory.mkdir(parents=True, exist_ok=True)
    evidence = dict(info, artifacts=[{'name': artifact.name, 'bytes': artifact.stat().st_size, 'sha256': hashlib.sha256(artifact.read_bytes()).hexdigest()}])
    evidence['builtAt'] = datetime.datetime.now(datetime.timezone.utc).isoformat()
    target = directory / (info['buildNumber'] + '.json')
    with target.open('x', encoding='utf-8') as f: json.dump(evidence, f, indent=2)
    return target

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--root', type=Path, default=ROOT)
    sub = parser.add_subparsers(dest='action', required=True)
    for command in ('freeze', 'promote', 'validate', 'prepare', 'record'):
        p = sub.add_parser(command)
        if command in ('freeze', 'promote'): p.add_argument('--approval', required=True)
        if command != 'freeze': p.add_argument('--freeze', required=True)
        if command == 'promote': p.add_argument('--verification', required=True)
        if command == 'prepare': p.add_argument('--output', required=True); p.add_argument('--build-number', required=True)
        if command == 'record': p.add_argument('--build-info', required=True); p.add_argument('--artifact', required=True)
        if command == 'validate':
            p.add_argument('--props'); p.add_argument('--definitions'); p.add_argument('--display'); p.add_argument('--numeric'); p.add_argument('--customer')
    a = parser.parse_args(); root = a.root.resolve()
    if a.action == 'freeze': print(allocate(root, a.approval))
    elif a.action == 'promote': print(allocate(root, a.approval, a.freeze, a.verification))
    elif a.action == 'validate':
        record = validate_props(root, a.freeze, a.props) if a.props else validate(root, a.freeze)
        if a.definitions: validate_definitions(root, a.freeze, a.definitions)
        for actual, expected in ((a.display, record['customerVersion']), (a.customer, record['customerVersion']), (a.numeric, record['numericVersion'])):
            if actual is not None and actual != expected: raise ValueError('Effective build version mismatch')
        print('PASS Freeze ' + record['customerVersion'])
    elif a.action == 'prepare': print('PASS prepared ' + prepare(root, a.freeze, a.output, a.build_number)['customerVersion'])
    else: print(record_build(root, a.freeze, a.build_info, a.artifact))

if __name__ == '__main__':
    try: main()
    except (ValueError, OSError, KeyError, subprocess.CalledProcessError) as error:
        print('BLOCKED: ' + str(error), file=sys.stderr); sys.exit(1)
