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
    
    Documents documents = new Documents(apiClient);
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
  
  Documents documents = new Documents(apiClient);
  Document document = documents.Find("id");
  
  //save the original file
  documents.SaveFile(document.Id, "path/to/save/file.pdf");
  //save the signed xml file
  documents.SaveXml(document.Id, "path/to/save/xml.xml");
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

Template methods:
- Create Template

```csharp
  using MifielAPI.Dao;
  using MifielAPI.Objects; 

  //Template template = new Template();
  //template.Track -> Boolean	Optional true if you want documents generated from this template to be endorsable
  //template.Type -> String	Optional (Required if param track is true) For now, the only value is 'promissory-note’ (pagaré)
  //template.Name -> String	The name of the template
  //template.Description -> String	Optional Internal description of your template
  //template.content -> String The Content of the PDF (in TEXT/HTML format)
 
  var content = new StringBuilder("<div>");
  content.AppendLine();
  content.Append("Name <field name='name' type='text'>NAME</field>");
  content.AppendLine();
  content.Append("Date <field name='date' type='text'>DATE</field>");
  content.AppendLine();
  content.Append("</div>");

  var template = new Template()
  {
    Name = "New Template-" + Guid.NewGuid().ToString(),
    Description = "Confidential disclosure agreement",
    Content = content
  };

  var templatesDao = new Templates(apiClient);
  var template = templatesDao.save(template); // return MifielAPI.Objects.Template
```

- Get a Specific Template
```csharp
  using MifielAPI.Dao;
  using MifielAPI.Objects;

  var templatesDao = new Templates(apiClient);
  var template = templatesDao.Find("29f3cb01-744d-4eae-8718-213aec8a1678"); //must send the TemplateId/return MifielAPI.Objects.Template 
```

- Get All Templates
```csharp
  using MifielAPI.Dao;
  using MifielAPI.Objects;

  var templatesDao = new Templates(apiClient);
  var templates = templatesDao.FindAll(); //return List<MifielAPI.Objects.Template> 
```

- Delete Template
```csharp
  using MifielAPI.Dao;
  using MifielAPI.Objects;
  using System.Collections.Generic;
  
  var templatesDao = new Templates(apiClient);
  templatesDao.Delete("29f3cb01-744d-4eae-8718-213aec8a1678"); // must send the TemplateId
```

- Template Fields
```csharp
  using MifielAPI.Dao;
  using MifielAPI.Objects;
  using System.Collections.Generic;
  
  var templatesDao = new Templates(apiClient);
  var fields = templatesDao.GetFields("29f3cb01-744d-4eae-8718-213aec8a1678"); //must send the TemplateId / return List<MifielAPI.Objects.TemplateFields> 
```

- Template documents
```csharp
  using MifielAPI.Dao;
  using MifielAPI.Objects;
  using System.Collections.Generic;
  
  var templatesDao = new Templates(apiClient);
  var documents = templatesDao.GetDocuments("29f3cb01-744d-4eae-8718-213aec8a1678"); //must send the TemplateId / return List<MifielAPI.Objects.TemplateDocuments> 
```

- Generate a document from a template
```csharp
  using MifielAPI.Dao;
  using MifielAPI.Objects;
  using System.Collections.Generic;
  
  //var templateGenerateDocument = new TemplateGenerateDocument();
  //templateGenerateDocument.Track	-> Boolean	Optional true if you want your document to be endorsable
  //templateGenerateDocument.Type	  -> String	Optional (Required if param track is true) For now, the only value is 'promissory-note’ (pagaré)
  //templateGenerateDocument.Name	  -> String	Optional The name of the document
  //templateGenerateDocument.Fields	-> JSON [Hash]	A hash with the fields {name: value}
  //templateGenerateDocument.Signatures	-> Array[Signatory] A list of Signatory Object
  //templateGenerateDocument.CallbackUrl ->String	Optional A Callback URL to post when the document gets signed
  //templateGenerateDocument.SignCallbackUrl -> String Optinal A Callback URL to post every time someone signs
  //templateGenerateDocument.ExternalId	-> String unique id for you to identify the document in the response or fetch it
  //templateGenerateDocument.ManualClose -> Boolean Indicates that the closing of the document will be manual (for example 2 of 3 signed)
  
  var data = new TemplateDocumentsData()
  {
    Fields = new Dictionary<string, string>
      {
        { "name", "My Client Name" },
        { "date", System.DateTime.Today.ToShortDateString() }
      },
    CallbackUrl = "https://www.example.com/webhook/url",
    ExternalId = Guid.NewGuid().ToString(),
    SignCallbackUrl = "https://www.example.com/webhook/sign",
    ManualClose = false,
    Signatures = new List<Signature>(){
      new Signature(){
          Email = "juan+carlos+zavala+lopez@mifiel.com",
          TaxId = "ZACA850805JX8",
          SignerName = "Carlos Zavala Lopez"
      }
    }
  };

  var templateGenarateDoc = new TemplateGenerateDocument()
  {
    Id = template.Id,
    Name = "document",
    Data = data
  };

  var templatesDao = new Templates(apiClient);
  var document = templatesDao.GenerateDocument(templateGenarateDoc); // return MifielAPI.Objects.Document
```

- Generate several documents from a template
```csharp
using MifielAPI.Dao;
using MifielAPI.Objects;
using System.Collections.Generic;
  
var documents = new List<TemplateDocumentsData> {
  new TemplateDocumentsData(){
        Signatures = new List<Signature>(){
          new Signature(){
            Email = "juan+carlos+zavala+lopez@mifiel.com",
            TaxId = "ZACA850805JX8",
            SignerName = "Carlos Zavala Lopez"
          }
    },
    Fields = new Dictionary<string, string>
      {
        { "name", "My Client Name" },
        { "date", System.DateTime.Today.ToShortDateString() }
      },
    SignCallbackUrl = "https://www.example.com/webhook/sign",
    CallbackUrl = "https://www.example.com/webhook/url",
    ExternalId = Guid.NewGuid().ToString(),
    ManualClose = false
  },
  new TemplateDocumentsData()
    {
      Signatures = new List<Signature>(){
        new Signature(){
          Email = "ja.zavala.aguilar@gmail.com",
          TaxId = "ZAAJ8301061E=",
          SignerName = "Juan Zavala Aguilar"
          }
    },
    Fields =  new Dictionary<string, string>
      {
        { "name", "My Client Name" },
        { "date", System.DateTime.Today.ToShortDateString() }
      },
    SignCallbackUrl = "https://www.example.com/webhook/sign",
    CallbackUrl = "https://www.example.com/webhook/url",
    ExternalId = Guid.NewGuid().ToString(),
    ManualClose = false
  }
};

var templateGenarateDocs = new TemplateGenerateDocuments()
{
  TemplateId = template.Id,
  Identifier = Guid.NewGuid().ToString(),
  Documents = documents,
  CallbackUrl = "https://www.example.com/webhook/url",
};
var templatesDao = new Templates(apiClient);
var results = templatesDao.GenerateSeveralDocuments(templateGenarateDocs); // return MifielAPI.Objects.SimpleResponse
```