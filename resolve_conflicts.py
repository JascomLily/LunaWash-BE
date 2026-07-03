import re

def resolve_file(path, keep='both'):
    with open(path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Simple regex to find conflict blocks
    pattern = re.compile(r'<<<<<<< HEAD\n(.*?)\n=======\n(.*?)\n>>>>>>> [a-f0-9]+', re.DOTALL)
    
    def replacer(match):
        head = match.group(1)
        remote = match.group(2)
        if keep == 'both':
            return head + '\n' + remote
        elif keep == 'head':
            return head
        elif keep == 'remote':
            return remote
    
    resolved = pattern.sub(replacer, content)
    
    with open(path, 'w', encoding='utf-8') as f:
        f.write(resolved)

# Resolve AuthService (keep head to retain the stack trace logic)
resolve_file(r'd:\SWP\LunaWash-BE\LunaWash.BLL\Services\AuthService.cs', 'head')

# Resolve ApplicationDbContext (keep both since one is Attendance, other is ServicePackage)
resolve_file(r'd:\SWP\LunaWash-BE\LunaWash.DAL\Data\ApplicationDbContext.cs', 'both')

# Resolve Snapshot (keep both)
resolve_file(r'd:\SWP\LunaWash-BE\LunaWash.DAL\Migrations\ApplicationDbContextModelSnapshot.cs', 'both')

