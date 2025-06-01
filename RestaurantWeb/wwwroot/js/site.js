// Массив изображений для слайдера
const heroImages = [
    "/images/slide-01.jpg",
    "/images/slide-02.jpg",
    "/images/slide-03.jpg"
];

let currentImageIndex = 0;
const heroImage = document.getElementById('hero-image');

// Предзагрузка изображений
function preloadImages() {
    heroImages.forEach(img => {
        const image = new Image();
        image.src = img;
    });
}

// Плавная смена изображения
function changeImageWithFade() {
    heroImage.style.opacity = 0; // Начинаем исчезание

    setTimeout(() => {
        currentImageIndex = (currentImageIndex + 1) % heroImages.length;
        heroImage.src = heroImages[currentImageIndex];
        heroImage.style.opacity = 1; // Появляем новое изображение
    }, 500); // Половина времени анимации (должно совпадать с CSS)
}

// Инициализация
preloadImages();

// CSS-переход для плавности
heroImage.style.transition = 'opacity 0.5s ease-in-out';

// Запускаем слайдер
setInterval(changeImageWithFade, 3000);