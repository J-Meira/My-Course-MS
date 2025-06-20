import { Navbar } from "@/components";

import { SignalRProvider, ToasterProvider } from "@/providers";
import { getCurrentUser } from "@/services";

import "./globals.css";

export const metadata = {
  title: "Live Auctions",
  description: "Auction for cars",
};

export default async function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const user = await getCurrentUser();
  return (
    <html lang="en">
      <body>
        <Navbar />
        <main className="container mx-auto px-5 pt-10">
          <SignalRProvider user={user}>{children}</SignalRProvider>
        </main>
        <ToasterProvider />
      </body>
    </html>
  );
}
