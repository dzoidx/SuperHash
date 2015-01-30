# SuperHash

Threaded big file hash computation helper. Divides file to the parts and computes hash for that parts independently in thread. The max speed for specific device equals to its storage and CPU performance.

.NET 4.0 needed.

# Tests

SSD
File size: 5053465272<br/>
time: 24932 ms<br/>
speed: 0,20 Gb/s<br/>
[Not threaded hash]<br/>
time: 55769 ms<br/>
speed: 0,09 Gb/s<br/>

HDD
File size: 5053465272<br/>
time: 103602 ms<br/>
speed: 0,05 Gb/s<br/>
[Not threaded hash]<br/>
time: 185602 ms<br/>
speed: 0,03 Gb/s<br/>

# Usage
```
var hasher = new SuperHash<SHA1Managed>(args[0]);
sh.Finished += chunks =>
               {
                   // Process chunks
               };
hasher.Start();
```
