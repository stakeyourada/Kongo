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
  Kongo --pool-name "0.7.0 Nightly" --pool-id "c5f0f12633dab1dcd69d4d0cc50d64d8e3625eaa3e7592c940819563a7e483b8" --rest-uri "http://127.0.0.1:3101" --database-path "k:\nightly\kongo.SQlite" --server.urls "http://localhost:5100;http://localhost:5101"
```

Example running as just web application, data collection disabled (--rest-url is required but will be ignored)
```code
  Kongo --pool-name "0.7.0 Nightly" --pool-id "c5f0f12633dab1dcd69d4d0cc50d64d8e3625eaa3e7592c940819563a7e483b8" --rest-uri "http://127.0.0.1:3101" --database-path "k:\nightly\kongo.SQlite" --server.urls "http://*:5102" --disable-all-collectors
```


## Command line options

  **Required switches**
  
    -p, --pool-name              example --pool-name "StakeYourAda.com"
    -r, --rest-uri               example --rest-uri "http://127.0.0.1:3101"
    -d, --database-path          example --database-path "x:\kongo\nightly"
                                         --database-path "/mnt/kongo/nightly"
  
  **Optional switches**
  
    --pool-id                    example --pool-id 8f779ef637831eb2acea6b3f9b3dbe4feb6e1d4ff49a06ef8bbec0d93a16db14
    --server.urls                example --server.urls http://localhost:5100;http://localhost:5101;http://*:5102

    --disable-all-collectors     Disable all data collection workers
    --disable-node-stats         Disable collection of node statistics every 30 seconds
    --disable-network-stats      Disable collection of network stats every 30 seconds
    --disable-fragment-logs      Disable collection of fragment logs every 30 seconds
    --show-fragments             If fragment logs are being collected this will show a verbose list of all fragment logs in the console output every 30 seconds
    --disable-leader-logs        Disable collection of leaders every 30 seconds
    --disable-pool-settings      Disable collection of pool settings every 30 seconds
    --disable-stake-pool-logs    Disable collection of list of Stake Pools every 30 seconds
    --disable-stake-logs         Disable collection of Stake Distribution logs every 30 seconds

    -v, --verbose                Verbose output (will show details of Rest calls, full reponse and json content returned)
    --help                       Display a help screen.
    --version                    Display version information.
  
