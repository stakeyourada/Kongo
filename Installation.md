# Kongo Installation and Use



Download latest release from [Kongo Releases](https://github.com/stakeyourada/Kongo/releases)

## Windows

  * Download release to your desired location and unzip
 
    **note:** This should be a location which can access the rest endpoint for your stakepool node. e.g. if your node-config.yaml is configured with the default lopback then you would need to install Kongo on the same machine.  Take care to not change your nodes rest endpoint and open it up to the internet, as that will expose your node to potential attacks.
  
  ```code
    rest:
      listen: "127.0.0.1:3101"
  ```
  
  * Edit [Kongo.Options.json](https://github.com/stakeyourada/Kongo/blob/master/src/Kongo/Kongo.options.json) and configure settings
  * Run Kongo.exe
```code
  PS C:\kongo> .\Kongo.exe
```


## Ubuntu / macOs

  * Download release to your desired location and unzip, untar
    
    **note:** This should be a location which can access the rest endpoint for your stakepool node. e.g. if your node-config.yaml is configured with the default lopback then you would need to install Kongo on the same machine.  Take care to not change your nodes rest endpoint to the internet which will expose your node to potential attacks.
  
  ```code
    rest:
      listen: "127.0.0.1:3101"
  ```  
  
  * Edit [Kongo.Options.json](https://github.com/stakeyourada/Kongo/blob/master/src/Kongo/Kongo.options.json) and configure settings
  * chmod +x Kongo
  * Run Kongo
```code
steve@/home/steve/kongo:~$ Kongo
```

## Kongo command line examples

**Note:** Command line arguments are optional and will override any settings in [Kongo.Options.json](https://github.com/stakeyourada/Kongo/blob/master/src/Kongo/Kongo.options.json)

Example running two instances side by side
```code
  Kongo --pool-name "0.8.0-rc2 Nightly" --pool-id "89d415166e5040fd56ef5cf9afbf4a1c1f2e1964ac9498e86c022259f391daff" --database-path "d:/nightly/kongo.SQlite" --server.urls "http://localhost:5100"
  
  Kongo --pool-name "0.8.0-rc2 Beta" --pool-id "8f779ef637831eb2acea6b3f9b3dbe4feb6e1d4ff49a06ef8bbec0d93a16db14" --database-path "d:/beta/kongo.SQlite" --server.urls "http://localhost:5200"

```

## Command line options

All switches will be populated from [Kongo.Options.json](https://github.com/stakeyourada/Kongo/blob/master/src/Kongo/Kongo.options.json) if not specified

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
