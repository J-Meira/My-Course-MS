import type { NextConfig } from "next";
import withFlowbiteReact from "flowbite-react/plugin/nextjs";

const nextConfig: NextConfig = {
  experimental: {
    serverActions: {},
  },
  images: {
    domains: ["cdn.pixabay.com"],
  },
  output: "standalone",
};

export default withFlowbiteReact(nextConfig);