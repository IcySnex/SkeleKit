<template>
  <section class="landing-hero">
    <div class="landing-grid" aria-hidden="true" />

    <div class="landing-content">
      <NuxtLinkLocale
        to="https://github.com/IcySnex/SkeleKit/releases/tag/v0.1.0"
        target="_blank"
        class="release-pill"
      >
        <SmartIcon name="lucide:sparkles" :size="14" />
        SkeleKit 0.1.0 is available
        <SmartIcon name="lucide:arrow-right" :size="14" />
      </NuxtLinkLocale>

      <h1>Native iOS UI.<br><span>Just C#.</span></h1>
      <p>
        A C# UI framework for native iOS: real UIKit controls, no storyboards or constraints, zero boilerplate.
      </p>

      <div class="landing-actions">
        <NuxtLinkLocale to="/getting-started/introduction">
          <UiButton>
            Get started
            <SmartIcon name="lucide:arrow-right" class="ml-1" :size="16" />
          </UiButton>
        </NuxtLinkLocale>
        <NuxtLinkLocale to="https://github.com/IcySnex/SkeleKit" target="_blank">
          <UiButton variant="outline">
            <SmartIcon name="lucide:github" class="mr-1" :size="16" />
            GitHub
          </UiButton>
        </NuxtLinkLocale>
      </div>
    </div>
  </section>
</template>

<style scoped>
.landing-hero {
  position: relative;
  isolation: isolate;
  min-height: 620px;
  overflow: hidden;
  display: grid;
  place-items: center;
  padding: 6rem 1.5rem 4rem;
}

.landing-grid {
  position: absolute;
  z-index: -1;
  inset: 0 -10%;
  background-image:
    linear-gradient(to right, hsl(var(--foreground) / 0.14) 1px, transparent 1px),
    linear-gradient(to bottom, hsl(var(--foreground) / 0.14) 1px, transparent 1px);
  background-size: 64px 64px;
  mask-image: radial-gradient(ellipse 82% 70% at 50% 38%, black 12%, transparent 82%);
  -webkit-mask-image: radial-gradient(ellipse 82% 70% at 50% 38%, black 12%, transparent 82%);
}

.landing-grid::after {
  content: '';
  position: absolute;
  inset: 0;
  background:
    radial-gradient(circle at 45% 38%, hsl(var(--brand) / 0.1), transparent 34%),
    radial-gradient(circle at 57% 40%, hsl(var(--brand-secondary) / 0.055), transparent 32%),
    radial-gradient(circle at 50% 42%, transparent 10%, hsl(var(--background) / 0.55) 72%, hsl(var(--background)) 100%);
}

.landing-content {
  display: flex;
  width: min(850px, 100%);
  flex-direction: column;
  align-items: center;
  text-align: center;
}

.release-pill {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  border: 1px solid hsl(var(--border));
  border-radius: 999px;
  background: hsl(var(--background) / 0.76);
  padding: 0.4rem 0.75rem;
  color: hsl(var(--muted-foreground));
  font-size: 0.8125rem;
  font-weight: 500;
  backdrop-filter: blur(10px);
  transition: color 150ms ease, border-color 150ms ease;
}

.release-pill:hover {
  color: hsl(var(--foreground));
  border-color: hsl(var(--foreground) / 0.24);
}

.release-pill :deep(svg:first-child) {
  color: hsl(var(--brand));
}

h1 {
  margin-top: 1.5rem;
  max-width: 720px;
  font-size: clamp(2.75rem, 6vw, 4rem);
  font-weight: 650;
  line-height: 1;
  letter-spacing: -0.045em;
}

h1 span {
  color: hsl(var(--brand));
}

p {
  margin-top: 1.75rem;
  max-width: 640px;
  color: hsl(var(--muted-foreground));
  font-size: 1rem;
  font-weight: 400;
  line-height: 1.6;
}

.landing-actions {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 0.75rem;
  margin-top: 2rem;
}

@media (max-width: 640px) {
  .landing-hero {
    min-height: 560px;
    padding-top: 4rem;
  }

  .landing-grid {
    background-size: 44px 44px;
  }
}
</style>
