import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Cibra Agro - Análise Inteligente",
  description: "Sistema de análise inteligente de postagens agrícolas com IA",
  keywords: "agricultura, IA, análise, plantio, colheita, agronegócio",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="pt-BR">
      <body className={inter.className}>{children}</body>
    </html>
  );
}