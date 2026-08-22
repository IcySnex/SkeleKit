# Releasing SkeleKit

The CI workflow builds and validates the framework, gallery, checked-in template, library package,
and template package on every pull request and push to `main`. A tag matching the package version
publishes the already-validated artifacts to NuGet.org and creates a GitHub release.

## One-time NuGet.org setup

1. Sign in to NuGet.org and make sure the intended account or organization will own both
   `SkeleKit.iOS` and `SkeleKit.Templates`.
2. Under **Trusted Publishing**, create a GitHub policy with:
   - repository owner: `IcySnex`
   - repository: `SkeleKit`
   - workflow file: `ci.yml`
   - environment: leave empty
3. In the GitHub repository, add an Actions secret named `NUGET_USER`. Its value is the NuGet.org
   profile name used by the trusted-publishing policy, not an email address.

Trusted publishing exchanges GitHub's OIDC identity for a short-lived NuGet API key during the
release job. No long-lived NuGet API key needs to be stored in GitHub.

## Release a version

1. Update `SkeleKitPackageVersion` in `Directory.Build.props`.
2. Update the fallback `SkeleKitPackageVersion` in
   `Samples/SkeleKit.Template/SkeleKit.Template.csproj` to the same value. CI rejects a mismatch.
3. Update `PackageReleaseNotes` in both package projects and the README for user-visible changes,
   then commit and push `main`.
4. Wait for CI to pass.
5. Create and push the matching tag. For version `0.1.0`:

   ```bash
   git tag -a v0.1.0 -m "SkeleKit 0.1.0"
   git push origin v0.1.0
   ```

The tag must be exactly `v<PackageVersion>`. The workflow publishes `SkeleKit.iOS`, its `.snupkg`,
and `SkeleKit.Templates`, then attaches all packages to the generated GitHub release.

NuGet package IDs and versions are immutable after publication. For a rehearsal, run the CI workflow
manually or download its `nuget-packages` artifact; do not push a test build with the final stable
version to NuGet.org.
