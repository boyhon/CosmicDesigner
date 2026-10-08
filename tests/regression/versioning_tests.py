"""TC-070-001: real isolated Git fixtures; never allocate a release for this checkout."""
from pathlib import Path
import importlib.util, json, subprocess, tempfile, unittest, xml.etree.ElementTree as ET, sys, shutil
from unittest.mock import patch

ROOT = Path(__file__).resolve().parents[2]
spec = importlib.util.spec_from_file_location('versioning', ROOT / 'release/versioning.py')
v = importlib.util.module_from_spec(spec); spec.loader.exec_module(v)

class VersionTests(unittest.TestCase):
    def setUp(self):
        (ROOT / 'artifacts').mkdir(exist_ok=True)
        self.temp = tempfile.TemporaryDirectory(prefix='version-test-', dir=ROOT / 'artifacts')
        self.root = Path(self.temp.name)
        assert self.root.resolve().is_relative_to((ROOT / 'artifacts').resolve())
        (self.root / 'release').mkdir(); (self.root / 'release/Version.props').write_bytes((ROOT / 'release/Version.props').read_bytes())
        (self.root / '.gitignore').write_text('artifacts/\nrelease/freezes/\nrelease/builds/\n**/bin/\n**/obj/\n**/__pycache__/\n')
        (self.root / 'source.cs').write_text('source 1')
        (self.root / 'docs/project/change-requests').mkdir(parents=True)
        (self.root / 'docs/project/change-requests/CR-070.md').write_text('fixture')
        (self.root / 'tests/regression').mkdir(parents=True)
        (self.root / 'tests/regression/manifest.json').write_text(json.dumps({'cases': [dict(testId='T-A',status='ACTIVE',executionType='AUTO'),dict(testId='T-M',status='ACTIVE',executionType='MANUAL')]}))
        for args in [('init',),('config','user.email','fixture@example.invalid'),('config','user.name','Test fixture'),('add','.'),('commit','-m','fixture')]:
            subprocess.run(['git',*args], cwd=self.root, check=True, capture_output=True)
        self.approval = ROOT / 'artifacts' / (self.root.name + '-approval.txt')
        self.approval.write_text('TEST FIXTURE ONLY: explicit simulated user Freeze approval',encoding='utf-8')
    def tearDown(self):
        self.approval.unlink(); self.temp.cleanup()
    def test_rebuild_and_inputs(self):
        with self.assertRaises(ValueError): v.allocate(self.root, None)
        f = v.allocate(self.root, self.approval)
        original = f.read_bytes()
        a = v.prepare(self.root,f,self.root/'artifacts/a','001')
        b = v.prepare(self.root,f,self.root/'artifacts/b','002')
        self.assertEqual(a['customerVersion'],'1.0.0-rc.1'); self.assertEqual(a,b); self.assertEqual(f.read_bytes(),original)
        self.assertEqual(len(list((self.root/'release/freezes').glob('*.json'))),1)
        v.validate_props(self.root,f,self.root/'artifacts/a/Freeze.props')
        definitions=self.root/'artifacts/a/ReleaseVersion.iss';v.validate_definitions(self.root,f,definitions)
        definitions.write_text(definitions.read_text().replace('1.0.0-rc.1','9.0.0'))
        with self.assertRaisesRegex(ValueError,'definitions mismatch'):v.validate_definitions(self.root,f,definitions)
        with patch.object(v,'sdk',return_value='different-sdk'):
            with self.assertRaisesRegex(ValueError,'toolchain mismatch'): v.validate(self.root,f)
        # A commit containing only immutable ledger evidence retains the original source anchor.
        subprocess.run(['git','add','-f','release/freezes'],cwd=self.root,check=True,capture_output=True)
        subprocess.run(['git','commit','-m','ledger only'],cwd=self.root,check=True,capture_output=True)
        v.validate(self.root,f)
        (self.root/'source.cs').write_text('modified existing CR; no new CR')
        with self.assertRaisesRegex(ValueError,'input mismatch'): v.prepare(self.root,f,self.root/'artifacts/c','003')
        with self.assertRaisesRegex(ValueError,'dirty'): v.allocate(self.root,self.approval)
        (self.root/'source.cs').write_text('source 1')
        (self.root/'new.cs').write_text('added')
        with self.assertRaisesRegex(ValueError,'input mismatch'): v.validate(self.root,f)
        (self.root/'new.cs').unlink()
        props=self.root/'artifacts/a/Freeze.props'; tree=ET.parse(props); tree.find('.//VCuttingCustomerVersion').text='9.0.0'; tree.write(props)
        with self.assertRaisesRegex(ValueError,'property mismatch'): v.validate_props(self.root,f,props)
        tampered=json.loads(f.read_text());tampered['record']['customerVersion']='9.0.0';f.write_text(json.dumps(tampered))
        with self.assertRaisesRegex(ValueError,'integrity'): v.validate(self.root,f)
    def test_sequence_promotion_and_evidence(self):
        f1=v.allocate(self.root,self.approval)
        f2=v.allocate(self.root,self.approval)
        self.assertEqual(v.read_record(f2)['customerVersion'],'1.0.0-rc.2')
        self.assertLess(tuple(map(int,v.numeric('1.0.0-rc.9',1).split('.'))),tuple(map(int,v.numeric('1.0.0-rc.10',1).split('.'))))
        with self.assertRaises(ValueError):v.allocate(self.root,self.approval,f2)
        evidence=self.root/'artifacts/verification.json';evidence.parent.mkdir(exist_ok=True)
        e=dict(candidateVersion='1.0.0-rc.2',sourceCommit=v.git(self.root,'rev-parse','HEAD'),automated=[dict(testId='T-A',result='PASS')],human=[dict(testId='T-M',result='PASS')])
        evidence.write_text(json.dumps(e))
        with self.assertRaisesRegex(ValueError,'reviewer'):v.allocate(self.root,self.approval,f2,evidence)
        e['human'][0].update(reviewer='SIMULATED QA',observedAt='SIMULATED',evidence='FIXTURE ONLY');evidence.write_text(json.dumps(e))
        final=v.allocate(self.root,self.approval,f2,evidence)
        self.assertEqual(v.read_record(final)['customerVersion'],'1.0.0')
        self.assertLess(tuple(map(int,v.numeric('1.0.0-rc.2',1).split('.'))),tuple(map(int,v.numeric('1.0.0',1).split('.'))))
        self.assertLess(tuple(map(int,'1.20.29.0'.split('.'))),tuple(map(int,v.numeric('1.0.0-rc.1',1).split('.'))))
        self.assertLess(tuple(map(int,v.numeric('1.0.0',1).split('.'))),tuple(map(int,v.numeric('1.0.1-rc.1',1).split('.'))))
        with self.assertRaises(ValueError):v.allocate(self.root,self.approval)
        v.prepare(self.root,f2,self.root/'artifacts/build','001')
        exe=self.root/'artifacts/test.exe';exe.write_bytes(b'fixture artifact')
        info=self.root/'artifacts/build/build.json';original=info.read_text();changed=json.loads(original);changed['numericVersion']='9.0.0.0';info.write_text(json.dumps(changed))
        with self.assertRaisesRegex(ValueError,'evidence/Freeze mismatch'):v.record_build(self.root,f2,info,exe)
        info.write_text(original)
        record=v.record_build(self.root,f2,self.root/'artifacts/build/build.json',exe)
        self.assertEqual(json.loads(record.read_text())['artifacts'][0]['sha256'],v.hashlib.sha256(b'fixture artifact').hexdigest())
        with self.assertRaises(FileExistsError):v.record_build(self.root,f2,self.root/'artifacts/build/build.json',exe)
        config=self.root/'release/Version.props';tree=ET.parse(config);tree.find('.//TargetCustomerVersion').text='0.9.0';tree.write(config)
        subprocess.run(['git','add','release/Version.props'],cwd=self.root,check=True,capture_output=True)
        subprocess.run(['git','commit','-m','lower target fixture'],cwd=self.root,check=True,capture_output=True)
        with self.assertRaisesRegex(ValueError,'advance'): v.allocate(self.root,self.approval)
        tree.find('.//WindowsVersionEpoch').text='2';tree.write(config)
        subprocess.run(['git','add','release/Version.props'],cwd=self.root,check=True,capture_output=True)
        subprocess.run(['git','commit','-m','changed epoch fixture'],cwd=self.root,check=True,capture_output=True)
        with self.assertRaisesRegex(ValueError,'epoch is immutable'): v.allocate(self.root,self.approval)
    def test_invalid_mapping(self):
        for version in ('1.0-rc1','1.0.0-rc.0','1.0.0-rc.65535','01.0.0','65535.0.0'):
            with self.assertRaises(ValueError):v.numeric(version,1)
    def test_real_frozen_build_and_effective_override_gate(self):
        # Copy the actual build integration into an isolated repository, then Freeze only that fixture.
        for name in ('Directory.Build.props','Directory.Build.targets','NuGet.Config','Shared/VersionInfo.cs','release/versioning.py'):
            target=self.root/name;target.parent.mkdir(parents=True,exist_ok=True);shutil.copyfile(ROOT/name,target)
        project=self.root/'Fixture/Fixture.csproj';project.parent.mkdir()
        project.write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>WinExe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup></Project>')
        (project.parent/'Program.cs').write_text('using System.Reflection; using VCuttingRelease; Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(new { display=VersionInfo.Display, development=VersionInfo.Development, build=VersionInfo.Build, about=VersionInfo.About("Fixture"), numeric=Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()!.Version }));')
        subprocess.run(['git','add','.'],cwd=self.root,check=True,capture_output=True)
        subprocess.run(['git','commit','-m','build fixture'],cwd=self.root,check=True,capture_output=True)
        freeze=v.allocate(self.root,self.approval);out=self.root/'artifacts/frozen';v.prepare(self.root,freeze,out,'compile-001')
        command=['dotnet','build',str(project),'-c','Release','-r','win-x64','-p:SelfContained=true','-p:BaseOutputPath='+str(self.root/'artifacts/app')+'/', '-p:VCuttingFreezeProps='+str(out/'Freeze.props'),'-p:VCuttingReleasePython='+sys.executable,'--configfile',str(self.root/'NuGet.Config'),'-v','quiet']
        cache=ROOT/'.dotnet-home/.nuget/packages'
        if cache.exists(): command.append('-p:RestorePackagesPath='+str(cache))
        result=subprocess.run(command,cwd=self.root,capture_output=True,text=True,encoding='utf-8',errors='replace',timeout=180)
        self.assertEqual(result.returncode,0,result.stdout+result.stderr)
        dll=self.root/'artifacts/app/Release/net10.0/win-x64/Fixture.dll'
        info=json.loads(subprocess.check_output(['dotnet',str(dll)],cwd=self.root,text=True,encoding='utf-8'))
        self.assertEqual(info['display'],'1.0.0-rc.1');self.assertEqual(info['numeric'],'2.0.0.1');self.assertEqual(info['development'],'1.20.29')
        self.assertTrue(info['build'].startswith(v.read_record(freeze)['sourceCommit']))
        self.assertIn('1.0.0-rc.1',info['about'])
        self.assertEqual((dll.parent/'help/js/version.js').read_text(encoding='utf-8-sig').strip(),'window.VCuttingVersion = "1.0.0-rc.1";')
        bad=subprocess.run(command+['-p:VCuttingDisplayVersion=9.0.0'],cwd=self.root,capture_output=True,text=True,encoding='utf-8',errors='replace',timeout=90)
        self.assertNotEqual(bad.returncode,0);self.assertIn('Effective build version mismatch',bad.stdout+bad.stderr)
        (project.parent/'Program.cs').write_text('changed existing code')
        bad=subprocess.run(command,cwd=self.root,capture_output=True,text=True,encoding='utf-8',errors='replace',timeout=90)
        self.assertNotEqual(bad.returncode,0);self.assertIn('Freeze input mismatch',bad.stdout+bad.stderr)

if __name__=='__main__': unittest.main()
