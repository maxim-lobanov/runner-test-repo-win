#!/usr/bin/python3

import os

from codecs import open
from setuptools import setup, find_packages

here = os.path.abspath(os.path.dirname(__file__))

about = {}
with open(os.path.join(here, 'src', 'umsg', '__about__.py'), 'r') as fp:
    exec(fp.read(), about)

with open(os.path.join(here, 'README.rst'), 'r') as fp:
    readme = fp.read()

required = []

setup(
    name=about['__title__'],
    version=about['__version__'],
    description=about['__description__'],
    long_description=readme,
    long_description_content_type='text/x-rst',
    author=about['__author__'],
    author_email=about['__author_email__'],
    classifiers=[
        'Development Status :: 5 - Production/Stable',
        'Intended Audience :: Developers',
        'License :: OSI Approved :: GNU Lesser General Public License v3 or later (LGPLv3+)',
        'Operating System :: OS Independent',
        'Programming Language :: Python',
        'Programming Language :: Python :: 3',
        'Programming Language :: Python :: 3 :: Only',
        'Programming Language :: Python :: 3.5',
        'Programming Language :: Python :: 3.6',
        'Programming Language :: Python :: 3.7',
        'Programming Language :: Python :: 3.8',
        'Programming Language :: Python :: 3.9',
        'Programming Language :: Python :: Implementation :: PyPy',
        'Topic :: Software Development',
        'Topic :: Software Development :: Libraries',
        'Topic :: System :: Logging',
    ],
    package_dir={'': 'src'},
    packages=find_packages(
        where='src',
        exclude=['docs', 'tests']
    ),
    package_data={'': ['LICENSE', 'NOTICE']},
    include_package_data=True,
    python_requires=">=3.5",
    install_requires=required,
    license=about['__license__'],
    #zip_safe=False,
)
