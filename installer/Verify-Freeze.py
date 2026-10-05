"""Audit managed source inputs without printing candidate secret values (stdlib only)."""
from pathlib import Path
import subprocess, re, json, sys, zipfile

root = Path(__file__).resolve().parents[1]
raw = subprocess.check_output(['git', 'ls-files', '-c', '-o', '--exclude-standard', '-z'], cwd=root)
paths = sorted(set(p.decode('utf-8') for p in raw.split(b'\0') if p))
patterns = [
    ('private key', re.compile(r'-----BEGIN (?:RSA |EC |OPENSSH |DSA )?PRIVATE KEY-----')),
    ('GitHub token', re.compile(r'\b(?:gh[pousr]_[A-Za-z0-9]{30,}|github_pat_[A-Za-z0-9_]{40,})\b')),
    ('AWS key', re.compile(r'\bAKIA[A-Z0-9]{16}\b')),
    ('OpenAI key', re.compile(r'\bsk-(?:proj-)?[A-Za-z0-9_-]{30,}\b')),
    ('secret assignment', re.compile(r'''(?im)^\s*["']?(?:password|api_key|access_token|client_secret)["']?\s*[:=]\s*["'][^"'\s]{12,}["']''')),
    ('user absolute path', re.compile(r'(?i)C:[/\\]Users[/\\](?!Public\b|Default\b)[^/\\\s<>]+')),
]
issues=[]; checked=0
for name in paths:
    path=root/name
    if not path.is_file(): continue
    if path.suffix.lower() in ('.exe','.dll','.msi','.zip','.pfx','.p12','.key','.user','.suo','.tmp','.log'):
        issues.append((name,'excluded build/user/credential extension')); continue
    if path.name.startswith('~$') or any(p in ('bin','obj','.vs','cache') for p in path.parts):
        issues.append((name,'temporary/IDE output')); continue
    content=''
    if path.suffix.lower() in ('.docx','.pptx','.xlsx'):
        with zipfile.ZipFile(path) as z:
            content='\n'.join(z.read(n).decode('utf-8','replace') for n in z.namelist() if n.endswith('.xml'))
    else:
        try: content=path.read_text(encoding='utf-8-sig')
        except UnicodeError: continue
    checked+=1
    for label,pattern in patterns:
        # Scanner pattern definitions are expected evidence, never a real credential.
        if name=='installer/Verify-Freeze.py': continue
        if pattern.search(content): issues.append((name,label))
for project in root.glob('*/*.csproj'):
    text=project.read_text(encoding='utf-8-sig')
    for element in ('ProjectReference','Resource','ApplicationIcon'):
        values=re.findall(r'<'+element+r'\b[^>]*Include="([^"]+)"',text) if element!='ApplicationIcon' else re.findall(r'<ApplicationIcon>([^<]+)',text)
        for value in values:
            normalized=value.replace('\\','/')
            if '$(' in value: continue
            if not list(project.parent.glob(normalized)) if '*' in normalized else not (project.parent/normalized).exists():
                issues.append((project.relative_to(root).as_posix(),'missing '+element))
manifest=json.loads((root/'tests/regression/manifest.json').read_text(encoding='utf-8-sig'))
ids=[x['testId'] for x in manifest['cases']]
if len(ids)!=len(set(ids)): issues.append(('tests/regression/manifest.json','duplicate immutable IDs'))
print(f'Freeze audit: {len(paths)} managed candidates; {checked} text/Office sources scanned; {len(issues)} findings.')
for name,label in issues: print(f'REVIEW {label}: {name}')
sys.exit(bool(issues))
