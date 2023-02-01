/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "../../Pages/**/*.cshtml",
        "../../wwwroot/**/*.{html,js}",
    ],
    theme: {
        extend: {
            width: {
                '128': '32rem',
            }
        },
    },
    plugins: [
        require('@tailwindcss/forms'),
    ],
}
