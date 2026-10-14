/** @type {import('tailwindcss').Config} */
module.exports = {
    // Hugo handles content scanning via hugo_stats.json
    // Configuration is minimal since Hugo manages the integration
    theme: {
        extend: {
            fontFamily: {
                sans: ['Inter', 'ui-sans-serif', 'system-ui'],
                mono: ['JetBrains Mono', 'ui-monospace', 'monospace'],
            },
        },
    }
}
