@echo off

:: Set Datify environment variables system-wide
setx DatifyDevDb "User ID =lolade_onepeople;Password=sC7DMGnjLb9B5-M;Server=datify.postgres.database.azure.com;Port=5432;Database=DatifyRemoteDevDb;Include Error Detail = true" /M

setx DatifyLocalDb "User ID =test;Password=Qwerty1234.;Server=localhost;Port=5432;Database=DatifyLocalDevDb;Include Error Detail = true" /M

setx DatifyProdDb "123xyz" /M

setx DatifyAzureBlobStorage "DefaultEndpointsProtocol=https;AccountName=testdevcontainer;AccountKey=Wk60jYqT9LXb0nZWxRVvRIYF9SRHA6vLXV5fUDW8m+lQptsWIhZmLOvkGARFZRekNyNKdSzAKZIG+AStqjgFlQ==;EndpointSuffix=core.windows.net" /M

setx DatifyFlutterwaveProdSecretKey "123xyz" /M

setx DatifyFlutterwaveDevSecretKey "FLWSECK_TEST-7bc7abd1dc997394275759f2107a04ed-X" /M

setx DatifyAzureMapsSubscriptionKey "123456exampleValues" /M

setx DatifyFlutterwaveDevPublicKey "FLWPUBK_TEST-f1ea6f74705ea0373373c18dd8977ccc-X" /M

setx DatifyFlutterwaveProdPublicKey "123xyz" /M

setx DatifyBrevoUsername "78d366001@smtp-brevo.com" /M

setx DatifyBrevoPassword "rz7Q4CLpV3IYjy0v" /M

setx DatifyBrevoHost "smtp-relay.brevo.com" /M

setx DatifyBrevoPort "587" /M

echo Datify environment variables have been set successfully.
pause
