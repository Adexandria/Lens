## Lens
###### A REST API that features that include :
- Text to speech
- Text to document
- Text To an existing document

### Application
- Azure Computer Vison
- Azure Translate Speech
- Azure Storage Account(Blob storage)
- Swagger

### Packages
- Microsoft.AspNetCore
- Newtonsoft.Json
- MimeMapping
- Swashbackle.Aspnetcore
- Azure.Storage.Blobs

### Documentation
[Swagger](https://adelens.azurewebsites.net/index.html)

##### How it works
- Text to Speech: A text image is uploaded using the post method, it's uploaded into the blob storage then downloaded into byte of data. It's sent to the computer vision to be interpreted and the computer receives the data for processing.The processed data is sent as a ssml file to the azure translate speech to convert the data to an mp3 file.
- Text to document: A text image is uploaded using the post method then stored in the blob storage.The image is then converted into byte of data.This is sent to the computer vision for interpretation.The computer receives the data then process it. This data is then stored in a document file.
- Text to an existing document: A text image and a document are uploaded using the post method.The image is then converted into byte of data.This is sent to the computer vision for interpretation.The computer receives the data then process it. The data is extracted from the document and merged to the existing data(the data gotten from the text image). Thhis data is then stored in a document file.

###### Thank You.
