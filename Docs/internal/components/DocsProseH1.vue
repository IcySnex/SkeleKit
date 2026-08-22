<template>
  <h1 :id class="scroll-m-20 text-3xl font-semibold tracking-tight wrap-break-word">
    <NuxtLink v-if="generate" :to="`#${id}`">
      <slot />
    </NuxtLink>
    <slot v-else />
  </h1>
</template>

<script setup lang="ts">
const { id } = defineProps<{ id?: string }>();
const { headings } = useRuntimeConfig().public.mdc;
const generate = computed(() => id && (
  (typeof headings?.anchorLinks === 'boolean' && headings.anchorLinks)
  || (typeof headings?.anchorLinks === 'object' && headings.anchorLinks?.h1)
));
</script>
