# Copyright © 2020 R.A. Stern
# SPDX-License-Identifier: LGPL-3.0-or-later

import logging
import logging.handlers
import sys

from .__about__ import (__author__, __build__, __copyright__, __description__,
                        __license__, __title__, __version__)
from .core import _msg, log, init, add_handler, get_attr, set_attr
from .util import _caller
import umsg.mixins


__all__ = [
    '__author__',
    '__build__',
    '__copyright__',
    '__description__',
    '__license__',
    '__title__',
    '__version__',
    '_msg',
    'add_handler',
    'init',
    'get_attr',
    'log',
    'set_attr'
]
