namespace ReferenceManager.Responses;

record PagedResult<T>(IEnumerable<T> Items, int Total, int Limit, int Offset);
