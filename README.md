# csharp-api-client
Mifiel API Client for C#

C# SDK for [Mifiel](https://www.mifiel.com) API.
Please read our [documentation](http://docs.mifiel.com/) for instructions on how to start using the API.

## Installation
TODO

## Usage

For your convenience Mifiel offers a Sandbox environment where you can confidently test your code.

To start using the API in the Sandbox environment you need to first create an account at [sandbox.mifiel.com](https://sandbox.mifiel.com).

Once you have an account you will need an APP_ID and an APP_SECRET which you can generate in [sandbox.mifiel.com/access_tokens](https://sandbox.mifiel.com/access_tokens).

Then you can configure the library with:

```csharp
  using MifielAPI;

  ApiClient apiClient = new ApiClient(appId, appSecret);
  // if you want to use our sandbox environment use:
  apiClient.Url = "https://sandbox.mifiel.com";
```

Document methods:

- Find:

  ```csharp
    using MifielAPI.Dao;
    using MifielAPI.Objects;

    Documents documents = new Documents(apiClient);
    Document document = documents.Find("id");
    document.OriginalHash;
    document.File;
    document.FileSigned;
    // ...
  ```

- Find all:

  ```csharp
    using MifielAPI.Dao;
    using MifielAPI.Objects;
    using System.Collections.Generic;

    Documents documents = new Documents(apiClient);
    List<Document> allDocuments = documents.FindAll();
  ```

- Create:

> Use only **original_hash** if you dont want us to have the file.<br>
> Only **file** or **original_hash** must be provided.

  ```csharp
    using MifielAPI.Dao;
    using MifielAPI.Objects;
    using MifielAPI.Utils;
    using System.Collections.Generic;

    Documents documents = new Documents(_apiClient);
    Document document = new Document()
    {
      File = "path/to/my-file.pdf",
      Signatures = new List<Signature>()
      {
        new Signature()
        {
          SignatureStr = "Signer 1",
          Email = "signer1@email.com",
          TaxId = "AAA010101AAA"
        },
        new Signature()
        {
          SignatureStr = "Signer 2",
          Email = "signer2@email.com",
          TaxId = "AAA010102AAA"
        }
      }
    };

    documents.Save(document);

    // if you dont want us to have the PDF, you can just send us
    // the original_hash and the name of the document. Both are required
    Document document2 = new Document()
    {
      OriginalHash = MifielUtils.GetDocumentHash("path/to/my-file.pdf"),
      Signatures = ...
    }

    documents.Save(document2);
  ```

- Save Document related files

```csharp
  using MifielAPI.Dao;
  using MifielAPI.Objects;
  using MifielAPI.Utils;

  Documents documents = new Documents(apiClient);
  Document document = documents.Find("id");

  //save the original file
  documents.SaveFile(document.Id, "path/to/save/file.pdf");
  //save the signed xml file
  documents.SaveXml(document.Id, "path/to/save/xml.xml");

  //append pdf base64 in original xml (when document was created using the hash)
  MifielUtils.AppendPDFBase64InOriginalXml("path/to/file.pdf", "path/to/originalXml", "path/to/newXml");
```

- Delete

  ```csharp
    using MifielAPI.Dao;
    using MifielAPI.Objects;

    Documents documents = new Documents(apiClient);
    documents.Delete("id");
  ```

Certificate methods:

- Find:

  ```csharp
    using MifielAPI.Dao;
    using MifielAPI.Objects;

    Certificates certificates = new Certificates(apiClient);
    Certificate certificate = certificates.Find("id");
    certificate.CerHex;
    certificate.TypeOf;
    // ...
  ```

- Find all:

  ```csharp
    using MifielAPI.Dao;
    using MifielAPI.Objects;
    using System.Collections.Generic;

    Certificates certificates = new Certificates(apiClient);
    List<Certificate> allCertificates = certificates.FindAll();
  ```

- Create

  ```csharp
    using MifielAPI.Dao;
    using MifielAPI.Objects;

    Certificates certificates = new Certificates(apiClient);
    Certificate certificate = new Certificate();
    certificate.File = "path/to/my-certificate.cer";

    certificates.Save(certificate);
  ```

- Delete

  ```csharp
    using MifielAPI.Dao;
    using MifielAPI.Objects;

    Certificates certificates = new Certificates(apiClient);
    certificates.Delete("id");
  ```
  
- Sign

```csharp
    using MifielAPI.Dao;
    using MifielAPI.Objects;
    
    SignProperties signProperties = new SignProperties(); // MifielAPI.Objects;
    //The library accepts either the path of the file (PrivateKeyFullPath) or its bytes (EncriptedPrivateKeyData)
    signProperties.PrivateKeyFullPath = 'path/to/my-private-key.key';
    signProperties.EncriptedPrivateKeyData = File.ReadAllBytes('path/to/my-private-key.key');

    signProperties.PassPhrase = 'my-password-privateKey';
    signProperties.DocumentId = document.Id;
    signProperties.DocumentOriginalHash = document.OriginalHash;

    //The library accepts either the path of the file (CertificateFullPath) or its bytes (CertificateData)
    signProperties.CertificateFullPath = 'path/to/my-certificate.cer';
    signProperties.CertificateData = File.ReadAllBytes('path/to/my-certificate.cer');

    Documents document = new Documents(apiClient); //MifielAPI.Dao.Document;
    document.sign(signProperties); //the result will be MifielAPI.Objects.Document;
    
  ```