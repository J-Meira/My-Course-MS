"use server";

import { auth } from "@/auth";

export const getCurrentUser = async () => {
  try {
    const session = await auth();

    if (!session) return null;

    return session.user;
  } catch (error) {
    console.error(error);
    return null;
  }
};
