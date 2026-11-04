<script lang="ts" module>
	import type { Component } from 'svelte';

	export type UserMenuItem = {
		/** Label shown in the menu */
		label: string;

		/** Optional icon */
		icon?: Component;

		/** Optional keyboard shortcut */
		shortcut?: string;

		/** Whether the item is disabled */
		disabled?: boolean;

		/** Called when the item is selected */
		onSelect?: () => void;

		/** Whether to render a separator before this item */
		separator?: boolean;
	};

	export type UserMenuProps = {
		/** Display name */
		name?: string;

		/** Avatar image URL */
		image?: string;

		/** Avatar fallback text (e.g. initials) */
		fallback?: string;

		fallbackIcon?: Component;

		/** Menu items */
		items: UserMenuItem[];

		/** Dropdown alignment */
		align?: 'start' | 'center' | 'end';

		class?: string;
	};
</script>

<script lang="ts">
	import * as Avatar from '#lib/components/ui/avatar/index.js';
	import Button from '#lib/components/ui/button/button.svelte';
	import * as DropdownMenu from '#lib/components/ui/dropdown-menu/index.js';
	import UserIcon from '@lucide/svelte/icons/user';

	let {
		name = 'User',
		fallback = '?',
		image,
		fallbackIcon = UserIcon,
		items,
		align = 'end',
		class: className
	}: UserMenuProps = $props();
</script>

<DropdownMenu.Root>
	<DropdownMenu.Trigger>
		{#snippet child({ props })}
			<Button
				{...props}
				variant="ghost"
				size="icon"
				class={className}
				aria-label="Open user menu"
			>
				<Avatar.Root>
					{#if image}
						<Avatar.Image src={image} alt={name} />
					{/if}
					<Avatar.Fallback class="bg-transparent text-foreground">
						{@const FallbackIcon = fallbackIcon}
						{#if FallbackIcon}
							<FallbackIcon />
						{:else}
							{fallback}
						{/if}
					</Avatar.Fallback>
				</Avatar.Root>
			</Button>
		{/snippet}
	</DropdownMenu.Trigger>

	<DropdownMenu.Content {align}>
		{#if name}
			<DropdownMenu.Label>{name}</DropdownMenu.Label>
			<DropdownMenu.Separator />
		{/if}

		{#each items as item (item)}
			{#if item.separator && items.indexOf(item) > 0}
				<DropdownMenu.Separator />
			{/if}
			<DropdownMenu.Item disabled={item.disabled} onSelect={item.onSelect}>
				{#if item.icon}
					<item.icon />
				{/if}

				{item.label}

				{#if item.shortcut}
					<DropdownMenu.Shortcut>
						{item.shortcut}
					</DropdownMenu.Shortcut>
				{/if}
			</DropdownMenu.Item>
		{/each}
	</DropdownMenu.Content>
</DropdownMenu.Root>
