<template>
  <li>
    <div v-if="link.children">
      <template v-if="folderStyle === 'group'">
        <div
          class="text-foreground/70 mt-2 flex items-center gap-2 rounded-md px-2 text-xs font-semibold outline-none"
          :class="[link.navTruncate !== false && 'h-8']"
        >
          <NuxtLinkLocale v-if="hasIndex" :to="link._path" @click="isOpen = true">
            <LayoutAsideTreeItemButton :link />
          </NuxtLinkLocale>
          <LayoutAsideTreeItemButton v-else :link />
        </div>
        <LayoutAsideTree :links="children" :level="level" />
      </template>

      <template v-else-if="hasIndex">
        <div
          class="relative"
          :class="isMobile ? 'h-8 w-full' : 'h-7 w-fit max-w-full'"
        >
          <NuxtLinkLocale
            :to="link._path"
            class="aside-page-link text-foreground/80 hover:bg-muted hover:text-primary flex min-w-0 items-center gap-2 rounded-md px-2 pr-9 text-sm"
            :class="[
              isMobile ? 'h-8 w-full' : 'h-7 w-fit max-w-full',
              isActive && 'bg-muted !text-primary font-medium',
            ]"
            @click="isOpen = true"
          >
            <LayoutAsideTreeItemButton :link />
          </NuxtLinkLocale>
          <button
            type="button"
            class="text-foreground/80 hover:text-primary absolute inset-y-0 right-0 flex w-8 cursor-pointer items-center justify-center rounded-md"
            :aria-label="`${isOpen ? 'Collapse' : 'Expand'} ${link.title}`"
            @click="isOpen = !isOpen"
          >
            <SmartIcon
              name="lucide:chevron-down"
              class="transition-transform"
              :class="[!isOpen && '-rotate-90']"
            />
          </button>
        </div>
        <div v-show="isOpen">
          <LayoutAsideTree :links="children" :level="level + 1" />
        </div>
      </template>

      <template v-else>
        <button
          class="text-foreground/80 hover:bg-muted hover:text-primary flex cursor-pointer items-center gap-2 rounded-md px-2 text-left text-sm"
          :class="[
            isMobile ? 'w-full' : 'w-fit max-w-full',
            link.navTruncate === false
              ? 'min-h-8 py-1.5'
              : isMobile ? 'h-8' : 'h-7',
          ]"
          @click="isOpen = !isOpen"
        >
          <SmartIcon
            v-if="folderStyle === 'tree'"
            name="lucide:chevron-down"
            class="transition-transform"
            :class="[!isOpen && '-rotate-90']"
          />
          <LayoutAsideTreeItemButton :link />
          <SmartIcon
            v-if="folderStyle === 'default'"
            name="lucide:chevron-down"
            class="ml-auto transition-transform"
            :class="[!isOpen && '-rotate-90']"
          />
        </button>
        <div v-show="isOpen">
          <LayoutAsideTree :links="children" :level="level + 1" />
        </div>
      </template>
    </div>

    <NuxtLinkLocale
      v-else
      :to="link._path"
      class="text-foreground/80 hover:bg-muted hover:text-primary flex items-center gap-2 rounded-md p-2 text-sm"
      :class="[
        isActive && 'bg-muted !text-primary font-medium',
        link.navTruncate !== false && 'h-8',
      ]"
    >
      <LayoutAsideTreeItemButton :link />
    </NuxtLinkLocale>
  </li>
</template>

<script setup lang="ts">
import type { NavItem } from '@ztl-uwu/nuxt-content';

const { link, level } = defineProps<{
  link: NavItem;
  level: number;
}>();

const { collapse, collapseLevel, folderStyle: defaultFolderStyle } = useConfig().value.aside;
const collapsed = useCollapsedMap();
const route = useRoute();
const isMobile = inject<Readonly<Ref<boolean>>>('docsAsideMobile', ref(false));

const indexPage = computed(() => link.children?.find(child => (
  child._path === link._path && !child.children
)));
const hasIndex = computed(() => Boolean(indexPage.value));
const children = computed(() => link.children?.filter(child => child !== indexPage.value) ?? []);

function containsRoute(path: string) {
  return route.path === path || route.path.startsWith(`${path}/`);
}

function defaultOpen() {
  if (containsRoute(link._path)) {
    return true;
  }
  if (link.collapse !== undefined) {
    return !link.collapse;
  }

  return level < collapseLevel && !collapse;
}

const isOpen = ref(collapsed.value.get(link._path) || defaultOpen());

watch(isOpen, (value) => {
  collapsed.value.set(link._path, value);
});

watch(() => route.path, () => {
  if (containsRoute(link._path)) {
    isOpen.value = true;
  }
});

function normalizePath(path: string) {
  const normalized = path.replace(/\/+$/, '');
  return normalized === '' ? '/' : normalized;
}

const isActive = computed(() => normalizePath(link._path) === normalizePath(route.path));
const folderStyle = computed(() => link.sidebar?.style ?? defaultFolderStyle);
</script>
