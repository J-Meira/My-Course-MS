import { Navbar } from "@/components";
import { ToasterProvider } from "@/providers";

import "./globals.css";

export const metadata = {
  title: "Live Auctions",
  description: "Auction for cars",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>
        <Navbar />
        <main className="container mx-auto px-5 pt-10">{children}</main>
        <ToasterProvider />
      </body>
    </html>
  );
}
