<script lang="ts">
	import { resolve } from '$app/paths';
	import favicon from '#lib/assets/favicon.svg';
	import ThemeToggle from '#lib/components/ui/theme-toggle/theme-toggle.svelte';
	import UserMenu from '#lib/components/ui/user-menu/user-menu.svelte';
	import type { UserMenuItem } from '#lib/components/ui/user-menu/user-menu.svelte';
	import LanguageSwitcher from '#lib/components/ui/language-switcher/app-language-switcher.svelte';
	import SettingsIcon from '@lucide/svelte/icons/settings';
	import LogOutIcon from '@lucide/svelte/icons/log-out';

	export let siteName = 'SomnosSuite';
	/** @type {'/' | '/dashboard'} */
	export let href: '/' | '/dashboard' = '/dashboard';

	const navLinkClass =
		'text-muted-foreground transition duration-200 hover:text-foreground focus:text-foreground active:text-foreground lg:px-2';

	const items: UserMenuItem[] = [
		{
			label: 'Settings',
			icon: SettingsIcon,
			onSelect: () => console.log('settings')
		},
		{
			label: 'Log out',
			icon: LogOutIcon,
			separator: true,
			onSelect: () => console.log('logout')
		}
	];
</script>

<nav
	aria-label="Main Navigation"
	class="relative flex w-full flex-wrap items-center justify-between border-b border-border/60 bg-background/80 py-2 shadow backdrop-blur-xl lg:py-4"
>
	<div class="flex w-full flex-wrap items-center justify-between gap-3 px-3">
		<!-- Logo -->
		<a class="mx-2 my-1 flex items-center lg:mt-0 lg:mb-0" href={resolve(href)}>
			<img src={favicon} class="me-2 h-10" alt="Logo" loading="lazy" />
			<span class="font-semibold text-foreground">{siteName}</span>
		</a>

		<!-- Navigation links -->
		<ul class="list-style-none me-auto flex flex-col ps-0 lg:flex-row" data-twe-navbar-nav-ref>
			<li class="mb-4 lg:mb-0 lg:pe-2" data-twe-nav-item-ref>
				<a class={navLinkClass} href={resolve('/dashboard')} data-twe-nav-link-ref
					>Dashboard</a
				>
			</li>
			<li class="mb-4 lg:mb-0 lg:pe-2" data-twe-nav-item-ref>
				<a class={navLinkClass} href={resolve('/about')} data-twe-nav-link-ref>About</a>
			</li>
			<li class="mb-4 lg:mb-0 lg:pe-2" data-twe-nav-item-ref>
				<a class={navLinkClass} href={resolve('/auth/login')} data-twe-nav-link-ref>Login</a
				>
			</li>
			<li class="mb-4 lg:mb-0 lg:pe-2" data-twe-nav-item-ref>
				<a class={navLinkClass} href={resolve('/app/station')} data-twe-nav-link-ref
					>Station</a
				>
			</li>
		</ul>

		<ThemeToggle />
		<LanguageSwitcher />
		<UserMenu name="Max Mustermann" fallback="MM" image="" {items} />
	</div>
</nav>
