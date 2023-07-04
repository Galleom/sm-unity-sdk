// The Score Milk SDK Bridge is responsible for
// relaying frontend messages to the game

window.gameLoaded = () => {
	window.parent.postMessage({
		message: 'gameLoaded',
		data: {
			version: '0.2.2',
			platform: 'Unity'
		}
	}, "*");
};

window.addEventListener('load', async function () {
	window.addEventListener("message", (event) => {
		const { message, data } = event.data;

		if (typeof message === 'string') {
			window.unityInstance.SendMessage(
				'ScoreMilkManager',
				message,
				data ? JSON.stringify(data) : undefined
			)
		}
	});
});
