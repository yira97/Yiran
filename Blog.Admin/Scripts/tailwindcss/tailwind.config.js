/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "../../Views/**/*.cshtml",
        "../../wwwroot/**/*.{html,js}",
        "../../Areas/**/*.{cshtml,js}",
    ],
    theme: {
        extend: {
            fontFamily: {
                noto: "'Noto Sans SC', sans-serif"
            }
        },
    },
    plugins: [
        require('@tailwindcss/forms'),
    ],
}
