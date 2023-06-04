import os
import re
import sys
import shutil
from datetime import datetime

def replace_word_in_file(filename, new_libname):
    with open(filename,'r') as file:
        data = file.readlines()

        for (i,line) in enumerate(data):
            if "const string __DllName" in line:
                data[i] = "\tpublic const string libname = " + '"' + new_libname + '"' + ";" + "\n"
                return data

        print("error libname not updated")
        return data

def update_unity(new_libname):

    # filename = 'unity/Assets/Plugins/FFI.cs'
    filename = 'unity/Assets/Plugins/ClamFFI.cs'
    data = replace_word_in_file(filename, new_libname)
    with open(filename, 'w') as file:
        file.writelines(data)

    return

def copy_lib(libname, new_libname, is_release):

    for f in ["unity\Assets\Plugins", "unity\Assets\Plugins\lib"]:
        if not os.path.exists(f):
            os.makedirs(f)
    build_mode = "release" if is_release else "debug"

    new_libname += ".dll"
    libname += ".dll"
    src = "clam_ffi/clam_ffi/target/" + build_mode + "/" + libname
    dst = "unity/assets/plugins/lib/" + new_libname

    print("copying ", new_libname, " from ", src," to ", dst)
    shutil.copyfile(src, dst)
    return

def build_lib(build_is_release):
    os.chdir("clam_ffi/clam_ffi")
    command = "cargo build"
    if build_is_release:
        command += " --release"
    os.system(command)
    os.chdir("../../")
    return

def gen_cs_binding():
    os.chdir("clam_ffi/cs_bindgen")
    command = "cargo run"
    os.system(command)
    os.chdir("../../")
    return


def should_build_release():
    if len(sys.argv) != 2:
        return False

    build_mode = sys.argv[1].lower()
    return build_mode == "release"

def main():

    timestamp = f'{datetime.now():%Y-%m-%d %H:%M:%S%z}'
    timestamp = re.sub('[-:r"\s"]', '', timestamp)
    libname = "clam_ffi"
    new_libname = libname + "_" + timestamp
    is_release = should_build_release()
    build_lib(is_release)
    copy_lib(libname, new_libname, is_release)
    gen_cs_binding()
    update_unity(new_libname)

main()