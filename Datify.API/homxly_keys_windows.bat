@echo off

:: Set Homxly environment variables system-wide
setx HomxlyDevDb3 "User ID =lolade_onepeople;Password=sC7DMGnjLb9B5-M;Server=onepeopledb.postgres.database.azure.com;Port=5432;Database=HomxlyRemoteDevDb3;Include Error Detail = true" /M

setx HomxlyLocalDb "User ID =test;Password=Qwerty1234.;Server=localhost;Port=5432;Database=HomxlyLocalDevDb;Include Error Detail = true" /M

setx HomxlyProdDb "123xyz" /M

setx HomxlyAzureBlobStorage "DefaultEndpointsProtocol=https;AccountName=testdevcontainer;AccountKey=Wk60jYqT9LXb0nZWxRVvRIYF9SRHA6vLXV5fUDW8m+lQptsWIhZmLOvkGARFZRekNyNKdSzAKZIG+AStqjgFlQ==;EndpointSuffix=core.windows.net" /M

setx HomxlyFlutterwaveProdSecretKey "123xyz" /M

setx HomxlyFlutterwaveDevSecretKey "FLWSECK_TEST-7bc7abd1dc997394275759f2107a04ed-X" /M

setx HomxlyAzureMapsSubscriptionKey "123456exampleValues" /M

setx HomxlyFlutterwaveDevPublicKey "FLWPUBK_TEST-f1ea6f74705ea0373373c18dd8977ccc-X" /M

setx HomxlyFlutterwaveProdPublicKey "123xyz" /M

setx HomxlyBrevoUsername "78d366001@smtp-brevo.com" /M

setx HomxlyBrevoPassword "rz7Q4CLpV3IYjy0v" /M

setx HomxlyBrevoHost "smtp-relay.brevo.com" /M

setx HomxlyBrevoPort "587" /M

echo Homxly environment variables have been set successfully.
pause
