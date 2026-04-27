namespace ReferenceManager.Requests;

record AffiliationRequest(string Name, string Country, string City);
record AuthorRequest(string Name, string? Email, List<AffiliationRequest> Affiliations);
record PaperRequest(string Title, List<AuthorRequest> Authors, int Year, string? Abstract, string? Doi, string? Journal, string? Booktitle);
