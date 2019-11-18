
REM bind cert to our ssl endpoint
netsh http add sslcert ipport=0.0.0.0:%1 certhash=%2 appid={%3}

REM Map azure file share as storage location
if not exist k:\ (
   cmdkey /add:%4 /user:%5 /pass:%6
   net use K: \\%7 /persistent:Yes
)
