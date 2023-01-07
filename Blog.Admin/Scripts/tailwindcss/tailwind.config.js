/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "../../Views/**/*.cshtml",
        "../../wwwroot/**/*.{html,js}",
    ],
    theme: {
        extend: {},
    },
    plugins: [
        require('@tailwindcss/forms'),
    ],
}
