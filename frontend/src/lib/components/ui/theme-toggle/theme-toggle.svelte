<script lang="ts">
	import { browser } from '$app/env';
	import { Sun, Moon } from '@lucide/svelte/icons';
	import { Button } from '#lib/components/ui/button/index.ts';

	type Theme = 'light' | 'dark';
	const THEME_STORAGE_KEY = 'somnos-theme';

	let theme = $state<Theme>('light');

	function getInitialTheme(): Theme {
		if (!browser) {
			return 'light';
		}

		const storedTheme = localStorage.getItem(THEME_STORAGE_KEY);
		if (storedTheme === 'light' || storedTheme === 'dark') {
			return storedTheme;
		}

		return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
	}

	function applyTheme(nextTheme: Theme) {
		theme = nextTheme;

		if (!browser) {
			return;
		}

		document.documentElement.classList.toggle('dark', nextTheme === 'dark');
		localStorage.setItem(THEME_STORAGE_KEY, nextTheme);
	}

	function toggleTheme() {
		applyTheme(theme === 'dark' ? 'light' : 'dark');
	}

	if (browser) {
		applyTheme(getInitialTheme());
	}
</script>

<Button
	onclick={toggleTheme}
	size="icon"
	variant="outline"
	aria-label={theme === 'dark' ? 'Switch to light mode' : 'Switch to dark mode'}
	title={theme === 'dark' ? 'Switch to light mode' : 'Switch to dark mode'}
>
	{#if theme === 'dark'}
		<Sun />
	{:else}
		<Moon fill="currentColor" />
	{/if}
</Button>
