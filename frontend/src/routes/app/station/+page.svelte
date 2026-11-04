<script lang="ts">
	import SessionStartForm from '#lib/components/SessionStartForm.svelte';
	import AnimalRecording from '#lib/components/AnimalRecording.svelte';
	import type { ProductionSession } from '#lib/types/ProductionSession.ts';

	let productionSession = $state<ProductionSession | null>(null);

	function handleSessionStarted(newSession: ProductionSession) {
		productionSession = newSession;
	}
</script>

<svelte:head>
	<title>Station</title>
</svelte:head>

{#if productionSession === null}
	<SessionStartForm onSessionStarted={handleSessionStarted} />
{:else}
	<AnimalRecording {productionSession} />
{/if}
