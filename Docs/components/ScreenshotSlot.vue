<template>
  <figure class="screenshot-card">
    <div class="screenshot-frame" :class="`screenshot-frame-${screenshot.format}`">
      <div v-show="appearance === 'light'" class="screenshot-variant">
        <ProseImg
          :src="screenshot.lightSrc"
          :alt="`${screenshot.alt}, light appearance`"
          :zoom="true"
          class="gallery-screenshot"
        />
      </div>

      <div v-show="appearance === 'dark'" class="screenshot-variant">
        <ProseImg
          :src="screenshot.darkSrc"
          :alt="`${screenshot.alt}, dark appearance`"
          :zoom="true"
          class="gallery-screenshot"
        />
      </div>
    </div>

    <figcaption>
      <div class="caption-heading">
        <strong>{{ screenshot.title }}</strong>

        <button
          class="appearance-toggle"
          type="button"
          :aria-label="`Show ${nextAppearance} appearance`"
          :title="`Show ${nextAppearance} appearance`"
          @click="toggleAppearance"
        >
          <Icon :name="appearance === 'light' ? 'lucide:sun' : 'lucide:moon'" />
        </button>
      </div>

      <span>{{ screenshot.description }}</span>
    </figcaption>
  </figure>
</template>

<script setup lang="ts">
type Screenshot = {
  lightSrc: string;
  darkSrc: string;
  alt: string;
  title: string;
  description: string;
  format: 'ipad' | 'iphone';
};

const props = defineProps<{ screenshot: Screenshot }>();
const colorMode = useColorMode();

const appearance = ref<'light' | 'dark'>(colorMode.value === 'light' ? 'light' : 'dark');

const nextAppearance = computed(() => appearance.value === 'light' ? 'dark' : 'light');

function toggleAppearance() {
  appearance.value = nextAppearance.value;
}
</script>

<style scoped>
.screenshot-card {
  min-width: 0;
  overflow: hidden;
  border: 1px solid hsl(var(--border));
  border-radius: calc(var(--radius) + 0.25rem);
  background: hsl(var(--card));
}

.screenshot-frame {
  position: relative;
  display: grid;
  place-items: center;
  overflow: hidden;
  border-bottom: 1px solid hsl(var(--border));
  background: hsl(var(--muted) / 0.28);
}

.screenshot-frame-ipad {
  aspect-ratio: 59 / 41;
}

.screenshot-frame-iphone {
  aspect-ratio: 201 / 437;
}

.screenshot-variant {
  width: 100%;
  height: 100%;
}

.screenshot-variant :deep(button) {
  width: 100%;
  height: 100%;
}

.screenshot-frame :deep(.gallery-screenshot) {
  display: block;
  width: 100%;
  height: 100%;
  object-fit: cover;
}

figcaption {
  display: grid;
  gap: 0.25rem;
  padding: 0.875rem 1rem 1rem;
}

.caption-heading {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
}

figcaption strong {
  color: hsl(var(--foreground));
  font-size: 0.8125rem;
  font-weight: 550;
}

figcaption span {
  color: hsl(var(--muted-foreground));
  font-size: 0.75rem;
  line-height: 1.5;
}

.appearance-toggle {
  display: grid;
  flex: none;
  width: 1.875rem;
  height: 1.875rem;
  place-items: center;
  border: 1px solid hsl(var(--border));
  border-radius: 0.375rem;
  background: hsl(var(--muted) / 0.5);
  color: hsl(var(--muted-foreground));
  transition:
    color 120ms ease,
    background-color 120ms ease,
    box-shadow 120ms ease;
}

.appearance-toggle:hover {
  color: hsl(var(--foreground));
  background: hsl(var(--background));
  box-shadow: 0 1px 2px hsl(var(--foreground) / 0.08);
}

.appearance-toggle:focus-visible {
  outline: 2px solid hsl(var(--ring));
  outline-offset: 2px;
}

.appearance-toggle :deep(svg) {
  width: 0.875rem;
  height: 0.875rem;
}
</style>
