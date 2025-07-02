import { Navbar } from "@/components";

import { SignalRProvider, ToasterProvider } from "@/providers";

import "./globals.css";
import { SessionProvider } from "next-auth/react";

export const metadata = {
  title: "Live Auctions",
  description: "Auction for cars",
};

export default async function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>
        <SessionProvider>
          <Navbar />
          <main className="container mx-auto px-5 pt-10">
            <SignalRProvider>{children}</SignalRProvider>
          </main>
          <ToasterProvider />
        </SessionProvider>
      </body>
    </html>
  );
}
