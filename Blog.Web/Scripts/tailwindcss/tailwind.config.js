/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "../../Pages/**/*.cshtml",
        "../../wwwroot/**/*.{html,js}",
    ],
    theme: {
        extend: {},
    },
    plugins: [
        require('@tailwindcss/forms'),
    ],
}
