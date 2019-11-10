:: Map azure file share as storage location
if not exist x:\ (
   cmdkey /add:syastorage.file.core.windows.net /user:Azure\syastorage /pass:6pXz/ugmjkBxRDkucDDiKEaK9/KKbg7cklUPOPgytbTa8PUhMZ6K7AK2JkwaOWHZbONNaItxsPsSiVPSDS7uLw==
   net use X: \\syastorage.file.core.windows.net\bcstorage /persistent:Yes
)
