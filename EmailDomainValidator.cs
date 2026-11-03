namespace inpsNuGet;

/// <summary>
/// Validates an email's domain against allow-lists. Instance-based (formerly the static <c>Check.Email</c>),
/// so configuration isn't shared process-wide. Matching is case-insensitive. Not thread-safe while configuring.
/// </summary>
public sealed class EmailDomainValidator
{
    readonly HashSet<string> _names = new(StringComparer.OrdinalIgnoreCase);
    readonly HashSet<string> _extensions = new(StringComparer.OrdinalIgnoreCase);
    readonly HashSet<string> _domains = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>true: match the whole domain against <see cref="AddDomain"/> entries; false: name + extension mode.</summary>
    public bool UseFullDomain { get; set; }

    public EmailDomainValidator AddDomainName(string name) { _names.Add(name); return this; }
    public EmailDomainValidator AddDomainExtension(string extension) { _extensions.Add(extension); return this; }
    public EmailDomainValidator AddDomain(string domain) { _domains.Add(domain); return this; }

    /// <summary>
    /// Requires exactly one '@' and a non-empty local part. In name/extension mode the name is the text before the
    /// first dot and the extension is everything after it (so "company.co.uk" has extension "co.uk").
    /// </summary>
    public bool IsValid(string? email)
    {
        if (string.IsNullOrEmpty(email)) return false;
        int at = email.IndexOf('@');
        if (at <= 0 || at != email.LastIndexOf('@')) return false;

        ReadOnlySpan<char> domain = email.AsSpan(at + 1);
        if (domain.IsEmpty) return false;
        if (UseFullDomain) return _domains.Contains(domain.ToString());

        int dot = domain.IndexOf('.');
        if (dot <= 0 || dot == domain.Length - 1) return false;

        return _names.Contains(domain[..dot].ToString()) && _extensions.Contains(domain[(dot + 1)..].ToString());
    }
}
