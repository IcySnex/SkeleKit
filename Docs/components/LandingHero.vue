<template>
  <section class="landing-hero">
    <div class="landing-content">
      <div class="landing-copy">
        <h1>Native iOS UI<br><span>in C#.</span></h1>
        <p>
          A C# UI framework for native iOS: real UIKit controls, no storyboards or constraints, zero boilerplate.
        </p>

        <div class="landing-actions">
          <NuxtLinkLocale to="/getting-started/installation/using-template">
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

      <div class="landing-example">
        <slot />
      </div>
    </div>
  </section>
</template>

<style scoped>
.landing-hero {
  min-height: 620px;
  display: grid;
  place-items: center;
  padding: 6rem 1.5rem 4.5rem;
}

.landing-content {
  display: grid;
  width: min(1120px, 100%);
  grid-template-columns: minmax(0, 0.85fr) minmax(480px, 1.15fr);
  align-items: center;
  gap: clamp(3rem, 7vw, 6rem);
}

.landing-copy {
  min-width: 0;
}

h1 {
  max-width: 570px;
  font-size: clamp(2.75rem, 5.4vw, 4rem);
  font-weight: 650;
  line-height: 1.03;
  letter-spacing: -0.045em;
}

h1 span {
  color: hsl(var(--brand));
}

.landing-copy > p {
  max-width: 560px;
  margin-top: 1.5rem;
  color: hsl(var(--muted-foreground));
  font-size: 1rem;
  font-weight: 400;
  line-height: 1.65;
}

.landing-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  margin-top: 2rem;
}

.landing-example {
  min-width: 0;
}

.landing-example :deep(.relative.overflow-hidden) {
  margin: 0;
  border-color: hsl(var(--border));
  background: hsl(var(--card) / 0.92);
  box-shadow:
    0 10px 16px rgb(var(--landing-example-shadow) / 0.05);
}

.landing-example :deep(pre) {
  min-height: 370px;
  padding-top: 0.7rem;
  padding-bottom: 0.7rem;
  font-size: 0.8125rem;
  line-height: 1.65;
}

@media (max-width: 900px) {
  .landing-hero {
    padding-top: 5rem;
  }

  .landing-content {
    grid-template-columns: 1fr;
    gap: 3rem;
  }

  .landing-copy {
    max-width: 640px;
  }

  .landing-example :deep(pre) {
    min-height: auto;
  }
}

@media (max-width: 640px) {
  .landing-hero {
    min-height: auto;
    padding: 4rem 1rem 3.5rem;
  }

}
</style>
