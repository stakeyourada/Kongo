:: Map azure file share as storage location
if not exist k:\ (
   cmdkey /add:%1 /user:%2 /pass:%3
   net use K: \\%4 /persistent:Yes
)
