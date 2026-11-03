# Changelog

## [2.0.0] - 2026-11-03
- BREAKING: major cleanup release. See migrating-to-v2.md for every change and its replacement.
- Rename Convert to StringConvert (fixes the System.Convert name clash)
- Replace Check.Email with the instance-based EmailDomainValidator
- Replace Check.CheckConnection with async Check.HasInternetAsync
- Sort: every method returns a new array; add EducationalSort; fix several crashes and bugs
- Fix Zip Slip in SimpleFileHandler.ExtractZipSafe
- PyCS: lazy init, structured ProcessResult, TLS verification no longer disabled by default
- Add unit tests; CI now runs tests and publishes from main only
