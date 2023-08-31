# jar-file-cleaner
A simple C# tool for pruning jar files of unused/unnecessary code and resources from a jar file

I created this because I encountered a Maven build problem, where contents of an external library would be packaged into my resulting jar file, despite setting it as "provided".
After fighting with the clunky Java build system I decided to just give up on Java and brute-force the solution by writing this.

## Usage

  -i, --input        Path to the input JAR file

  -r, --resources    (Optional) Folder containing the resources, any files contained in this folder won't be removed, works by comparing paths relative to the root folder

  -p, --packages     List of packages to preserve, separated by ','. Eg 'com.google.guava,com.mydomain.mypackage'

  -v, --verbose      (Optional) Verbose logging, logs each file/folder it deletes (won't list all files contained in the folder if the entire folder is deleted)

  -o, --output       (Optional) The output JAR file, overwrites the original if unspecified

  --pause            Pauses execution before end, for debugging
