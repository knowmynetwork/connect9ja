#!/bin/bash

# Add environment variables to shell configuration
echo 'export DatifyDevDb="User ID =lolade_onepeople;Password=sC7DMGnjLb9B5-M;Server=datify.postgres.database.azure.com;Port=5432;Database=DatifyRemoteDevDb;Include Error Detail = true"' >> ~/.zshrc
echo 'export DatifyLocalDb="User ID =test;Password=Qwerty1234.;Server=localhost;Port=5432;Database=DatifyLocalDevDb;Include Error Detail = true"' >> ~/.zshrc
echo 'export DatifyProdDb="123xyz"' >> ~/.zshrc
echo 'export DatifyAzureBlobStorage="DefaultEndpointsProtocol=https;AccountName=testdevcontainer;AccountKey=Wk60jYqT9LXb0nZWxRVvRIYF9SRHA6vLXV5fUDW8m+lQptsWIhZmLOvkGARFZRekNyNKdSzAKZIG+AStqjgFlQ==;EndpointSuffix=core.windows.net"' >> ~/.zshrc
echo 'export DatifyFlutterwaveProdSecretKey="123xyz"' >> ~/.zshrc
echo 'export DatifyFlutterwaveDevSecretKey="FLWSECK_TEST-7bc7abd1dc997394275759f2107a04ed-X"' >> ~/.zshrc
echo 'export DatifyAzureMapsSubscriptionKey="123456exampleValues"' >> ~/.zshrc
echo 'export DatifyFlutterwaveDevPublicKey="FLWPUBK_TEST-f1ea6f74705ea0373373c18dd8977ccc-X"' >> ~/.zshrc
echo 'export DatifyFlutterwaveProdPublicKey="123xyz"' >> ~/.zshrc
echo 'export DatifyBrevoUsername="78d366001@smtp-brevo.com"' >> ~/.zshrc
echo 'export DatifyBrevoPassword="rz7Q4CLpV3IYjy0v"' >> ~/.zshrc
echo 'export DatifyBrevoHost="smtp-relay.brevo.com"' >> ~/.zshrc
echo 'export DatifyBrevoPort="587"' >> ~/.zshrc

# Reload shell configuration
source ~/.zshrc

echo "Environment variables have been set successfully."
