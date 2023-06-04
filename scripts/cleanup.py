import os
import glob

files = glob.glob("unity/Assets/Plugins/lib/*")
for f in files:
    print(f)
    os.remove(f)