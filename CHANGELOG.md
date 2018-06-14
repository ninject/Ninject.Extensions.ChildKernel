# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [3.3.0]

### Added
 - Support .NET Standard 2.0

### Removed
 - .NET 3.5, .NET 4.0 and Silverlight

## [3.2.0]

### Changed
- When choosing the constructor the child kernel will now regard dependencies fulfilled by any of the parent kernels.

## [3.0.0.0]

### Removed
- No web builds. All builds are have not reference to web anymore

### Changed
- Implicit bindngs are resolved on the child kernel and not the parent kernel anymore