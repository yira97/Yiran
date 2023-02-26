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
            },
            fontFamily: {
                noto: "'Noto Sans SC', sans-serif"
            }
        },
    },
    plugins: [
        require('@tailwindcss/forms'),
    ],
}
