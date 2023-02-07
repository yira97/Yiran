/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "../../Views/**/*.cshtml",
        "../../wwwroot/**/*.{html,js}",
        "../../Areas/**/*.{cshtml,js}",
    ],
    theme: {
        extend: {},
    },
    plugins: [
        require('@tailwindcss/forms'),
    ],
}
