# Reference Manager Software Evolution

## Evolution point 1:

### Expectation:
Expectation is an API with CRUD operations for Papers. It should have a SQLite db and use entity framework. Swagger should be available.

Feature model: `Infrastructure`, `Papers`, `Authors`

### **Prompt 1:**
I'm building a research paper reference manager API in .NET 9 with SQLite. Set up Entity Framework Core with a DbContext and initial migration. Add a Papers endpoint with full CRUD — each paper should have a title, list of authors, publication year, abstract, and DOI. Make sure the database is created on startup and the API is ready to run.

### **Prompt 2:**
the authors should not be a simple string rather a model with name, affiliations(university, country, city), email.

### **Prompt 3:**
authors can have multiple affiliations and can be companies as well as universities.

### **Prompt 4:**
Configure swagger so that the endpoints are easily accessible when run.

### **Prompt 5:**
fill the database with sample data. Use the bib/library from the writing vault.

### Resulting Feature Model:
``` feature-model
ReferenceManager
    Database
    ApiDocs
    Papers
        CreatePaper
        GetPaper
        UpdatePaper
        DeletePaper
```
---

## Evolution point 2:

### Expectation:
Papers can be organized into named collections. Many-to-many relationship between Papers and Collections.

Feature model: + `Collections`

### **Prompt 1:**
Add a Collections feature — a collection is a named group of papers with an optional description. Papers can belong to multiple collections and a collection can have many papers. Add full CRUD for collections and endpoints to add or remove papers from a collection.

### **Prompt 2:**
add a proper .gitignore for c# .net and structure project according to microsoft development guidelines.

### **Prompt 3:**
please continue with fixing the structure. Right now everything is in the monolithic program.cs file.
https://gist.github.com/alanwei43/247fdba7be30ee8f36117907109632a4
https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design

### **Prompt 4:**
Commit this and add some test data for collections. For example add a collection with papers that I have referenced in my accepted papers check vault for which papers have been accepted.

### **Prompt 5:**
add versioning (/api/v1/) and pagination to the list endpoints

### **Prompt 6:**
not sure if collections test data is wrong but I get this as the response when testing:
``` json
{
    "id":1,
    "name": "Accepted Papers",
    "description": "The Developer's published and accepted papers across SPLC, TSE, AISoLA, and Variability venues.",
    "papers": []
}
```

Additionally I would like to add some api tests using xunit so that we can see that we are not changing behaviours. No need for tests that save to the database we assume saving and reading from db works.

### Resulting Feature Model:
``` feature-model
ReferenceManager
    Database
    ApiDocs
    Versioning
    Papers
        CreatePaper
        GetPaper
        UpdatePaper
        DeletePaper
    Collections
        CreateCollection
        GetCollection
        UpdateCollection
        DeleteCollection
        AddPaperToCollection
        RemovePaperFromCollection
```
---

## Evolution point 3:

### Expectation:
Papers can be tagged with short labels. Many-to-many relationship between Papers and Tags.

Feature model: + `Tags`

### **Prompt 1:**
Add a Tags feature — tags are short labels that can be applied to papers. Papers can have multiple tags and tags can apply to multiple papers. Add endpoints to create and delete tags, list all tags, and add or remove tags from a paper.

### Resulting Feature Model
```
ReferenceManager
    Database
    ApiDocs
    Versioning
    Papers
        CreatePaper
        GetPaper
        UpdatePaper
        DeletePaper
    Collections
        CreateCollection
        GetCollection
        UpdateCollection
        DeleteCollection
        AddPaperToCollection
        RemovePaperFromCollection
    Tags
        CreateTag
        GetTag
        DeleteTag
        AddTagToPaper
        RemoveTagFromPaper
```

---

## Evolution point 4:

### Expectation:
Papers can be starred for quick access. Simple boolean flag with a dedicated list endpoint.

Feature model: + `Favorites`

### **Prompt 1:**
Add a Favorites feature — users should be able to star or unstar individual papers. Add an endpoint to toggle favorite status on a paper and an endpoint to list all favorited papers.

### Resulting Feature Model
```
ReferenceManager
    Database
    ApiDocs
    Versioning
    Papers
        CreatePaper
        GetPaper
        UpdatePaper
        DeletePaper
    Collections
        CreateCollection
        GetCollection
        UpdateCollection
        DeleteCollection
        AddPaperToCollection
        RemovePaperFromCollection
    Tags
        CreateTag
        GetTag
        DeleteTag
        AddTagToPaper
        RemoveTagFromPaper
    Favorites
        ToggleFavorite
        ListFavorites
```
---

## Evolution point 5 (MERGE):

### Expectation:

  Tags and Collections are removed and replaced by a single Groups feature. One Group model with name + optional description, one join table PaperGroup, one GroupsController. Existing tags migrated as groups without descriptions; existing collections migrated as groups with their descriptions. All paper-tagging and paper-grouping endpoints now go through /api/v1/groups. Tags and Collections endpoints no longer exist.

  Feature model: Tags + Collections → Groups

### **Prompt 1:**
Tags and Collections overlap — both associate papers with named groups. Merge them into a single Groups feature. A group has a name and an optional description. Papers can belong to multiple groups and a group can contain many papers. Replace the separate Tags and Collections controllers, models, and join tables with a single Groups controller and model. Migrate existing tags as groups without descriptions and existing collections as groups with their descriptions preserved.

### Resulting Feature Model
```
ReferenceManager
    Database
    ApiDocs
    Versioning
    Papers
        CreatePaper
        GetPaper
        UpdatePaper
        DeletePaper
    Groups
        CreateGroup
        GetGroup
        UpdateGroup
        DeleteGroup
        AddPaperToGroup
        RemovePaperFromGroup
    Favorites
        ToggleFavorite
        ListFavorites
```
---

## Evolution point 6:

### Expectation:
API accepts .bib file uploads and creates Paper records from BibTeX entries.

Feature model: + `BibTeX.Import`

### **Prompt 1:**
Add a BibTeX import feature — add an endpoint that accepts a .bib file upload, parses it, and creates Paper records from the BibTeX entries. Handle the common fields: title, author, year, abstract, doi, journal, booktitle. Skip entries that already exist based on DOI.

### **Prompt 2:**
if doi is null then we should look at title + authors to check for duplicates.

### Resulting Feature Model
```
ReferenceManager
    Database
    ApiDocs
    Versioning
    Papers
        CreatePaper
        GetPaper
        UpdatePaper
        DeletePaper
        ImportBibTex
    Groups
        CreateGroup
        GetGroup
        UpdateGroup
        DeleteGroup
        AddPaperToGroup
        RemovePaperFromGroup
    Favorites
        ToggleFavorite
        ListFavorites
```
---

## Evolution point 7:

### Expectation:
API can export papers to .bib format, optionally filtered by folder or label. BibTeX is now a parent feature with Import and Export as children.

Feature model: + `BibTeX.Export` (BibTeX now parent with `Import` and `Export` children)

### **Prompt 1:**
Add a BibTeX export feature — add an endpoint that returns selected papers as a valid .bib file. Support optional filtering by folder or label so users can export a subset of their library. The output should be importable into tools like Zotero or Mendeley.

### **Prompt 2:**
export seems to be wrong: paper with title An IDE Plugin for Clone Management in Software Product Lines is exported as misc but should be inproceedings.

### **Prompt 3:**
you are right db entry is wrong. Not export.

### **Prompt 4:**
Seems the import bibtex feature was annotated wrongly for this "PUT /papers/{id} accepts journal and booktitle " update. It should just be part of the update feature which it already is by being added to the request.

### Resulting Feature Model
```
ReferenceManager
    Database
    ApiDocs
    Versioning
    Papers
        CreatePaper
        GetPaper
        UpdatePaper
        DeletePaper
        ImportBibTex
        ExportBibTex
    Groups
        CreateGroup
        GetGroup
        UpdateGroup
        DeleteGroup
        AddPaperToGroup
        RemovePaperFromGroup
    Favorites
        ToggleFavorite
        ListFavorites
```
---

## Evolution point 8 (SPLIT):

### Expectation:
Authors are no longer embedded in Papers. They become standalone managed entities with deduplication. Papers reference Authors by ID. The feature model splits Papers.Authors into a separate Authors feature.

Feature model: `Papers.Authors` → `Authors` (standalone)

### **Prompt 1:**
The current Author model is embedded in Papers which causes duplication — the same author appears many times with slightly different spellings. Refactor Authors into standalone managed entities with their own CRUD endpoints. Papers should reference Authors by ID. Add logic that checks for an existing author by name before creating a new one to prevent duplicates. Migrate existing data.

### **Prompt 2:**
please fix the sample data so that I can test

### **Prompt 3:**
Add an endpoint to merge two duplicate authors — given two author IDs, reassign all paper references to one and delete the other.

### **Prompt 4:**
Also make sure when authors are merged that affiliations are appended if they are different. such that author x with affiliation bebas uni and author y with affiliation abbas uni becomes author x with affiliations bebas uni and abbas uni as two entires in that authors affiliation list.

### Resulting Feature Model
```
ReferenceManager
    Database
    ApiDocs
    Versioning
    Papers
        CreatePaper
        GetPaper
        UpdatePaper
        DeletePaper
        ImportBibTex
        ExportBibTex
    Authors
        CreateAuthor
        GetAuthor
        ListAuthors
        UpdateAuthor
        DeleteAuthor
        MergeAuthors
    Groups
        CreateGroup
        GetGroup
        UpdateGroup
        DeleteGroup
        AddPaperToGroup
        RemovePaperFromGroup
    Favorites
        ToggleFavorite
        ListFavorites
```
---

## Evolution point 9:

### Expectation:
Full-text search across title, abstract, and author names, with optional filters.

Feature model: + `Search`

### **Prompt 1:**
Add a full-text search feature — add a search endpoint that queries across paper titles, abstracts, and author names. Support optional filters for folder, label, and year range. Return results ranked by relevance.

### **Prompt 2:**
the response model here is a bit strange. When quering papers in any way I do not want to also see an authors papers. It is enough to see the authors name, email and affiliation.

### Resulting Feature Model
```
ReferenceManager
    Database
    ApiDocs
    Versioning
    Papers
        CreatePaper
        GetPaper
        UpdatePaper
        DeletePaper
        ImportBibTex
        ExportBibTex
        SearchPapers
    Authors
        CreateAuthor
        GetAuthor
        ListAuthors
        UpdateAuthor
        DeleteAuthor
        MergeAuthors
    Groups
        CreateGroup
        GetGroup
        UpdateGroup
        DeleteGroup
        AddPaperToGroup
        RemovePaperFromGroup
    Favorites
        ToggleFavorite
        ListFavorites
```
---

## Evolution point 10 (REMOVE):

### Expectation:
Favorites feature is removed. The Organize system with Folders makes it redundant — users can create a "Starred" folder. Feature model shrinks.

Feature model: − `Favorites`

### **Prompt 1:**
Remove the Favorites feature entirely — the Folders system already covers this use case and having both creates confusion. Users can create a folder called "Starred" for the same purpose. Remove all related endpoints, models, database columns, and migrations.

