#!/bin/bash

# Add environment variables to shell configuration
echo 'export HomxlyDevDb3="User ID =lolade_onepeople;Password=sC7DMGnjLb9B5-M;Server=onepeopledb.postgres.database.azure.com;Port=5432;Database=HomxlyRemoteDevDb3;Include Error Detail = true"' >> ~/.zshrc
echo 'export HomxlyLocalDb="User ID =test;Password=Qwerty1234.;Server=localhost;Port=5432;Database=HomxlyLocalDevDb;Include Error Detail = true"' >> ~/.zshrc
echo 'export HomxlyProdDb="123xyz"' >> ~/.zshrc
echo 'export HomxlyAzureBlobStorage="DefaultEndpointsProtocol=https;AccountName=testdevcontainer;AccountKey=Wk60jYqT9LXb0nZWxRVvRIYF9SRHA6vLXV5fUDW8m+lQptsWIhZmLOvkGARFZRekNyNKdSzAKZIG+AStqjgFlQ==;EndpointSuffix=core.windows.net"' >> ~/.zshrc
echo 'export HomxlyFlutterwaveProdSecretKey="123xyz"' >> ~/.zshrc
echo 'export HomxlyFlutterwaveDevSecretKey="FLWSECK_TEST-7bc7abd1dc997394275759f2107a04ed-X"' >> ~/.zshrc
echo 'export HomxlyAzureMapsSubscriptionKey="123456exampleValues"' >> ~/.zshrc
echo 'export HomxlyFlutterwaveDevPublicKey="FLWPUBK_TEST-f1ea6f74705ea0373373c18dd8977ccc-X"' >> ~/.zshrc
echo 'export HomxlyFlutterwaveProdPublicKey="123xyz"' >> ~/.zshrc
echo 'export HomxlyBrevoUsername="78d366001@smtp-brevo.com"' >> ~/.zshrc
echo 'export HomxlyBrevoPassword="rz7Q4CLpV3IYjy0v"' >> ~/.zshrc
echo 'export HomxlyBrevoHost="smtp-relay.brevo.com"' >> ~/.zshrc
echo 'export HomxlyBrevoPort="587"' >> ~/.zshrc

# Reload shell configuration
source ~/.zshrc

echo "Environment variables have been set successfully."
