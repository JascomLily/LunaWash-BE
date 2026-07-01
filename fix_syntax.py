path = r'd:\SWP\LunaWash-BE\LunaWash.DAL\Data\ApplicationDbContext.cs'
with open(path, 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace(
    '.HasConstraintName("FK_Attendances_Users");\n        modelBuilder.Entity<ServicePackage>(entity =>',
    '.HasConstraintName("FK_Attendances_Users");\n        });\n\n        modelBuilder.Entity<ServicePackage>(entity =>'
)

with open(path, 'w', encoding='utf-8') as f:
    f.write(content)

# For Snapshot, let's just delete the snapshot and recreate it since it's auto-generated anyway.
# Wait, let's just remove the conflict markers from Snapshot.
path2 = r'd:\SWP\LunaWash-BE\LunaWash.DAL\Migrations\ApplicationDbContextModelSnapshot.cs'
with open(path2, 'r', encoding='utf-8') as f:
    content2 = f.read()

content2 = content2.replace('<<<<<<< HEAD\n', '')
content2 = content2.replace('=======\n', '')
content2 = re.sub(r'>>>>>>> [a-f0-9]+\n', '', content2)

with open(path2, 'w', encoding='utf-8') as f:
    f.write(content2)

