interface ThemePreference {
  theme: string;
  radius: number;
}

const themeVersion = '2';

export default defineNuxtPlugin(() => {
  const version = useCookie<string>('skelekit-theme-version');

  if (version.value === themeVersion) {
    return;
  }

  const theme = useCookie<ThemePreference>('theme');
  theme.value = {
    theme: 'zinc',
    radius: 0.5,
  };
  version.value = themeVersion;
});
