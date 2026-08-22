<template>
  <UiScrollArea orientation="vertical" class="relative h-full overflow-hidden py-6 pr-6 text-sm md:pr-4" type="hover">
    <LayoutHeaderNavMobile v-if="isMobile" class="mb-5 border-b pb-2" />
    <LayoutSearchButton v-if="config.search.inAside" />
    <ul v-if="guideSections.length" class="flex flex-col gap-1 border-b pb-4">
      <li v-for="link in guideSections" :key="link._path">
        <NuxtLinkLocale
          :to="link.redirect ?? link._path"
          class="text-foreground/75 hover:bg-muted hover:text-foreground inline-flex h-7 w-fit max-w-full items-center gap-2 rounded-md px-2 text-[13px] font-normal"
          :class="routePath.startsWith(link._path) && 'bg-muted !text-foreground font-medium'"
        >
          <SmartIcon v-if="link.icon" :name="link.icon" :size="16" />
          {{ link.title }}
        </NuxtLinkLocale>
      </li>
    </ul>
    <LayoutAsideTree :links="tree" :level="0" :class="guideSections.length ? 'pt-4' : 'pt-1'" />
  </UiScrollArea>
</template>

<script setup lang="ts">
defineProps<{ isMobile: boolean }>();

const { navDirFromPath } = useContentHelpers();
const config = useConfig();
const { locale, defaultLocale, navigation } = useI18nDocs();
const routePath = computed(() => useRoute().path);

const guideSections = computed(() => {
  if (routePath.value.startsWith('/reference')) {
    return [];
  }

  return navigation.value.filter(link => (
    link._path === '/getting-started' || link._path === '/guides'
  ));
});

const tree = computed(() => {
  const path = useRoute().path.split('/');
  const leveledPath = path.splice(0, locale.value === defaultLocale ? 2 : 3).join('/');
  return navDirFromPath(leveledPath, navigation.value) ?? [];
});
</script>
