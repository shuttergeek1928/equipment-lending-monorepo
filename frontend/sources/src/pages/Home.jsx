import { useEffect, useMemo, useState } from "react";

const SLIDE_INTERVAL_MS = 5000;

export default function Home() {
  const slides = useMemo(
    () => [
      {
        id: 1,
        title: "Lab Equipment Lending",
        description:
          "Reserve oscilloscopes, multimeters, and other lab essentials in minutes.",
        image:
          "https://images.unsplash.com/photo-1518770660439-4636190af475?auto=format&fit=crop&w=1200&q=80",
      },
      {
        id: 2,
        title: "Sports Gear Library",
        description:
          "Checkout sports kits for university events and practice sessions.",
        image:
          "https://images.unsplash.com/photo-1508609349937-5ec4ae374ebf?auto=format&fit=crop&w=1200&q=80",
      },
      {
        id: 3,
        title: "Audio-Visual Resources",
        description:
          "Borrow projectors, cameras, and microphones for presentations.",
        image:
          "https://images.unsplash.com/photo-1459183885421-5cc683b8dbba?auto=format&fit=crop&w=1200&q=80",
      },
    ],
    []
  );

  const [activeIndex, setActiveIndex] = useState(0);

  useEffect(() => {
    const timer = setInterval(() => {
      setActiveIndex((index) => (index + 1) % slides.length);
    }, SLIDE_INTERVAL_MS);

    return () => clearInterval(timer);
  }, [slides.length]);

  const handleNavigate = (direction) => {
    setActiveIndex((prev) => {
      if (direction === "next") {
        return (prev + 1) % slides.length;
      }
      return (prev - 1 + slides.length) % slides.length;
    });
  };

  return (
    <div className="container-center home-page">
      <div className="home-page__content">
        <h1 className="text-large">
          Welcome to the School Equipment Lending Portal
        </h1>
        <p className="text-medium">
          Let’s make equipment sharing easier for everyone!
        </p>

        <div className="home-carousel">
          <button
            type="button"
            className="home-carousel__control"
            onClick={() => handleNavigate("prev")}
            aria-label="View previous highlight"
          >
            ‹
          </button>

          <div className="home-carousel__viewport">
            {slides.map((slide, index) => (
              <article
                key={slide.id}
                className={`home-carousel__slide ${
                  index === activeIndex ? "is-active" : ""
                }`}
                aria-hidden={index !== activeIndex}
              >
                <img src={slide.image} alt={slide.title} loading="lazy" />
                <div className="home-carousel__caption">
                  <h3>{slide.title}</h3>
                  <p>{slide.description}</p>
                </div>
              </article>
            ))}
          </div>

          <button
            type="button"
            className="home-carousel__control"
            onClick={() => handleNavigate("next")}
            aria-label="View next highlight"
          >
            ›
          </button>
        </div>

        <div className="home-carousel__dots" role="tablist" aria-label="Highlights">
          {slides.map((slide, index) => (
            <button
              key={slide.id}
              type="button"
              className={`home-carousel__dot ${
                index === activeIndex ? "is-active" : ""
              }`}
              onClick={() => setActiveIndex(index)}
              aria-label={`Show slide ${index + 1}: ${slide.title}`}
              aria-selected={index === activeIndex}
              role="tab"
            />
          ))}
        </div>
      </div>
    </div>
  );
}

