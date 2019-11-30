# Kongo Installation and Use



Download latest release from [Kongo Releases](https://github.com/stakeyourada/Kongo/releases)

## Windows

  * Download release to your desired location and unzip
  * Run Kongo with desired options


## Ubuntu / macOs

  * Download release to your desired location and unzip, untar
  * chmod +x Kongo
  * Run Kongo with desired options

## Kongo startup examples

Example running with all data collection
```code
  Kongo --pool-name "0.8.0-rc1 Nightly" --pool-id "c5f0f12633dab1dcd69d4d0cc50d64d8e3625eaa3e7592c940819563a7e483b8" --rest-uri "http://127.0.0.1:3101" --database-path "d:/nightly/kongo.SQlite" --server.urls "http://localhost:5100;http://localhost:5101"
```

Example running just as a web application, data collection disabled (--rest-url is required but will be ignored)
```code
  Kongo --pool-name "0.8.0-rc1 Nightly" --pool-id "c5f0f12633dab1dcd69d4d0cc50d64d8e3625eaa3e7592c940819563a7e483b8" --rest-uri "http://127.0.0.1:3101" --database-path "d:/nightly/kongo.SQlite" --server.urls "http://*:5102" --disable-all-collectors
```


## Command line options

  **Required switches**
  
    -p, --pool-name              example --pool-name "StakeYourAda.com"
    -r, --rest-uri               example --rest-uri "http://127.0.0.1:3101"
    -d, --database-path          example --database-path "d:/kongo/nightly"
                                         --database-path "/mnt/kongo/nightly"
  
  **Optional switches**
  
    --pool-id                    example --pool-id 89d415166e5040fd56ef5cf9afbf4a1c1f2e1964ac9498e86c022259f391daff
    --server.urls                example --server.urls http://localhost:5100;http://localhost:5101;http://*:5102

    --disable-all-collectors     Disable all data collection workers
    
    --verbose-node-stats         Show verbose request details for collection of node statistics every 30 seconds
    --verbose-network-stats      Show verbose request details for collection of network stats every 30 seconds
    --verbose-fragment-logs      Show verbose request details for collection of fragment logs every 30 seconds
    --show-all-fragments         Show verbose list of fragment logs every 30 seconds, if fragment logs are being collected
    --verbose-leader-logs        Show verbose request details for collection of leaders every 30 seconds
    --verbose-pool-settings      Show verbose request details for collection of pool settings every 30 seconds
    --verbose-stake-pool-logs    Show verbose request details for collection of list of Stake Pools every 30 seconds
    --verbose-stake-logs         Show verbose request details for collection of Stake Distribution logs every 30 seconds
    
    --server.urls                ASP.NET endpoint urls
    --verbose                    Show all Verbose output
    --help                       Display this help screen
