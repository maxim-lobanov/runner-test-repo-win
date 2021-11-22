# Copyright © 2020 R.A. Stern
# SPDX-License-Identifier: LGPL-3.0-or-later

import inspect
import logging



DEPTH = 2


# ------------------------------------------------------------------ Functions -
def _caller(root=False):
    depth = DEPTH

    # walk the stack, find the first ancestor that isn't us
    while True:
        try:
            s = inspect.stack()[depth]
            m = inspect.getmodule(s[0])
            depth += 1

            if not m.__name__.startswith('umsg.'):
                break
        except IndexError:
            break

    return m.__name__


def log_level(level):
    "Converts a string or integer level to proper logging.LEVEL"
    try:
        level = level.lower()
    except AttributeError:
        if level not in (10, 20, 30, 40, 50):
            return logging.INFO

    if level in ('crit', 'critical', logging.CRITICAL):
        return logging.CRITICAL
    elif level in ('error', logging.ERROR):
        return logging.ERROR
    elif level in ('warn', 'warning', logging.WARNING):
        return logging.WARNING
    elif level in ('debug', logging.DEBUG):
        return logging.DEBUG
    else:
        return logging.INFO
